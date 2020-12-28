using OctoAwesome.Client.Controls;
using System.Collections.Generic;
using engenious.Graphics;
using System.Linq;
using engenious;
using System;
using System.Windows.Threading;
using System.Threading;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private readonly Effect _simple;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly Texture2DArray _textures;
        private readonly Dispatcher _dispatcher;

        public static float OVERRIDE_LIGHT_LEVEL { get; set; }
        public static bool WIRE_FRAME { get; set; }

        /// <summary>
        /// Referenz auf den aktuellen Chunk (falls vorhanden)
        /// </summary>
        private IChunk _chunk;

        private bool _loaded = false; //CONTINUE https://youtu.be/CHYNA9865qQ?t=11058

        private VertexBuffer _vb;
        private static IndexBuffer ib;
        private int _vertexCount;
        private int _indexCount;
        private ILocalChunkCache _manager;

        private readonly SceneControl _sceneControl;
        private readonly IDefinitionManager _definitionManager;
        private static readonly RasterizerState _wireFrameState;
        
        /// <summary>
        /// Adresse des aktuellen Chunks
        /// </summary>
        public Index3? ChunkPosition
        {
            get => _chunkPosition;
            private set
            {
                _chunkPosition = value;
                NeedsUpdate = value != null;
            }
        }

        public bool DispatchRequired => Thread.CurrentThread.ManagedThreadId != _dispatcher.Thread.ManagedThreadId;

        static ChunkRenderer()
        {
            _wireFrameState = new RasterizerState() {FillMode = PolygonMode.Line, CullMode = CullMode.CounterClockwise};
            OVERRIDE_LIGHT_LEVEL = 0;
            WIRE_FRAME = false;
            
            _uvOffsets = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
        }

        public ChunkRenderer(SceneControl sceneControl, IDefinitionManager definitionManager, Effect simpleShader, GraphicsDevice graphicsDevice, Matrix projection, Texture2DArray textures)
        {
            _sceneControl = sceneControl;
            _definitionManager = definitionManager;
            _graphicsDevice = graphicsDevice;
            _textures = textures;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _simple = simpleShader;
            GenerateIndexBuffer();

            _vertices = new List<VertexPositionNormalTextureLight>();
            var textureColumns = _textures.Width / SceneControl.TEXTURESIZE;
            _textureWidth = 1f / textureColumns;
            _textureSizeGap = 1f / SceneControl.TEXTURESIZE;
            _textureGap = _textureSizeGap / 2;
            // BlockTypes sammlen
            var blockDefinitions = _definitionManager.GetBlockDefinitions();

            _textureOffsets = new Dictionary<IBlockDefinition, int>(blockDefinitions.Length);
            var definitionIndex = 0;

            foreach (var definition in definitionManager.GetBlockDefinitions())
            {
                var textureCount = definition.Textures.Count();
                _textureOffsets.Add(definition, definitionIndex);
                definitionIndex += textureCount;
            }
        }

        public void SetChunk(ILocalChunkCache manager, int x, int y, int z)
        {
            var newPosition = new Index3(x, y, z);

            if (_manager == manager && newPosition == ChunkPosition)
            {
                NeedsUpdate = !_loaded;
                return;
            }

            _manager = manager;
            ChunkPosition = newPosition;

            if (_chunk != null)
            {
                _chunk.Changed -= OnChunkChanged;
                _chunk = null;
            }

            _loaded = false;
            NeedsUpdate = true;
        }

        public bool NeedsUpdate = false;


        private void OnChunkChanged(IChunk c)
        {
            NeedsUpdate = true;
            _sceneControl.Enqueue(this);
        }

        public void Draw(Matrix view, Matrix projection, Index3 shift)
        {
            if (!_loaded)
                return;

            var worldViewProj = projection * view * Matrix.CreateTranslation(shift.X * Chunk.CHUNKSIZE_X, shift.Y * Chunk.CHUNKSIZE_Y, shift.Z * Chunk.CHUNKSIZE_Z);

            _simple.Parameters["OverrideLightLevel"].SetValue(OVERRIDE_LIGHT_LEVEL);
            _simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            _simple.Parameters["BlockTextures"].SetValue(_textures);

            _simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            _simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (_vb == null)
                    return;

                _graphicsDevice.RasterizerState = WIRE_FRAME ? _wireFrameState : RasterizerState.CullCounterClockwise;
                _graphicsDevice.VertexBuffer = _vb;
                _graphicsDevice.IndexBuffer = ib;

                foreach (var pass in _simple.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, _vertexCount, 0, _indexCount / 3);
                }
            }
        }

        private readonly object _ibLock = new object();
        private Index3? _chunkPosition;
        private float _textureWidth;
        private readonly float _textureSizeGap;
        private float _textureGap;
        private readonly Dictionary<IBlockDefinition, int> _textureOffsets;
        private static readonly Vector2[] _uvOffsets;
        private readonly List<VertexPositionNormalTextureLight> _vertices;

        public void GenerateIndexBuffer()
        {
            lock (_ibLock)
            {
                if (ib != null)
                    return;

                ib = new IndexBuffer(_graphicsDevice, DrawElementsType.UnsignedInt, Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Z * 6 * 6);
                var indices = new List<int>(ib.IndexCount);

                for (var i = 0; i < ib.IndexCount * 2 / 3; i += 4)
                {
                    indices.Add(i + 0);
                    indices.Add(i + 1);
                    indices.Add(i + 3);

                    indices.Add(i + 0);
                    indices.Add(i + 3);
                    indices.Add(i + 2);
                }

                ib.SetData(indices.ToArray());
            }
        }
#if DEBUG
        public bool RegenerateVertexBuffer()
#else
        public unsafe bool RegenerateVertexBuffer()
#endif
        {
            if (!ChunkPosition.HasValue)
                return false;

            // Chunk nachladen
            if (_chunk == null)
            {
                _chunk = _manager.GetChunk(ChunkPosition.Value);
                
                if (_chunk == null)
                {
                    //Thread.Sleep(10);
                    //RegenerateVertexBuffer();
                    //NeedsUpdate = false;
                    return false;
                }

                _chunk.Changed += OnChunkChanged;
            }

            var chunk = _chunk;
            
            _vertices.Clear();

#if DEBUG
            var blocks = new ushort[27];
#else
            var blocks = stackalloc ushort[27];
#endif

            var blockDefinitions = new IBlockDefinition[27];

            for (var z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        var block = chunk.GetBlock(x, y, z);

                        if (block == 0)
                            continue;

                        var blockDefinition = (IBlockDefinition) _definitionManager.GetDefinitionByIndex(block);

                        if (blockDefinition == null)
                            continue;

                        int textureIndex;

                        if (!_textureOffsets.TryGetValue(blockDefinition, out textureIndex))
                            continue;

                        for (var zOffset = -1; zOffset <= 1; zOffset++)
                        for (var yOffset = -1; yOffset <= 1; yOffset++)
                        for (var xOffset = -1; xOffset <= 1; xOffset++)
                        {
                            blocks[GetIndex(zOffset, yOffset, xOffset)] = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + xOffset, y + yOffset, z + zOffset));
                            blockDefinitions[GetIndex(zOffset, yOffset, xOffset)] =
                                (IBlockDefinition) _definitionManager.GetDefinitionByIndex(blocks[GetIndex(zOffset, yOffset, xOffset)]);
                        }

                        var topBlock = blocks[GetIndex(1, 0, 0)];
                        var topBlockDefintion = blockDefinitions[GetIndex(1, 0, 0)];
                        var bottomBlock = blocks[GetIndex(-1, 0, 0)];
                        var bottomBlockDefintion = blockDefinitions[GetIndex(-1, 0, 0)];
                        var southBlock = blocks[GetIndex(0, 1, 0)];
                        var southBlockDefintion = blockDefinitions[GetIndex(0, 1, 0)];
                        var northBlock = blocks[GetIndex(0, -1, 0)];
                        var northBlockDefintion = blockDefinitions[GetIndex(0, -1, 0)];
                        var westBlock = blocks[GetIndex(0, 0, -1)];
                        var westBlockDefintion = blockDefinitions[GetIndex(0, 0, -1)];
                        var eastBlock = blocks[GetIndex(0, 0, 1)];
                        var eastBlockDefintion = blockDefinitions[GetIndex(0, 0, 1)];

                        var globalX = x + chunk.Index.X * Chunk.CHUNKSIZE_X;
                        var globalY = y + chunk.Index.Y * Chunk.CHUNKSIZE_Y;
                        var globalZ = z + chunk.Index.Z * Chunk.CHUNKSIZE_Z;

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsSolidWall(Wall.Bottom) && topBlock != block))
                        {
                            var top = (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ));
                            var rotation = -blockDefinition.GetTextureRotation(Wall.Top, _manager, globalX, globalY, globalZ);
                            
                            var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(1, 0, -1), Wall.Left, GetIndex(1, 1, 0), Wall.Front);
                            var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 0, 1), Wall.Left, GetIndex(1, 1, 0), Wall.Front);
                            var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(1, 0, -1), Wall.Left, GetIndex(1, -1, 0), Wall.Front);
                            var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(1, 0, 1), Wall.Left, GetIndex(1, -1, 0), Wall.Front);

                            var vertYZ = new VertexPositionNormalTextureLight(
                                new Vector3(x + 0, y + 1, z + 1),
                                new Vector3(0, 0, 1),
                                _uvOffsets[(6 + rotation) % 4],
                                top,
                                AmbientToBrightness(valueYZ));
                            var vertXYZ = new VertexPositionNormalTextureLight(
                                new Vector3(x + 1, y + 1, z + 1),
                                new Vector3(0, 0, 1),
                                _uvOffsets[(7 + rotation) % 4],
                                top,
                                AmbientToBrightness(valueXYZ));
                            var vertZ = new VertexPositionNormalTextureLight(
                                new Vector3(x + 0, y + 0, z + 1),
                                new Vector3(0, 0, 1),
                                _uvOffsets[(5 + rotation) % 4],
                                top,
                                AmbientToBrightness(valueZ));
                            var vertXZ = new VertexPositionNormalTextureLight(
                                new Vector3(x + 1, y + 0, z + 1),
                                new Vector3(0, 0, 1),
                                _uvOffsets[(4 + rotation) % 4],
                                top,
                                AmbientToBrightness(valueXZ));

                            if (valueXYZ + valueZ <= valueYZ + valueXZ)
                            {
                                _vertices.Add(vertYZ);
                                _vertices.Add(vertXYZ);
                                _vertices.Add(vertZ);
                                _vertices.Add(vertXZ);
                            }
                            else
                            {
                                _vertices.Add(vertXYZ);
                                _vertices.Add(vertXZ);
                                _vertices.Add(vertYZ);
                                _vertices.Add(vertZ);
                            }
                        }
                        
                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsSolidWall(Wall.Top) && bottomBlock != block))
                        {
                            var bottom = (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ));
                            var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(-1, 0, -1), Wall.Left, GetIndex(-1, 1, 0), Wall.Front);
                            var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(-1, 0, 1), Wall.Left, GetIndex(-1, 1, 0), Wall.Front);
                            var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(-1, 0, -1), Wall.Left, GetIndex(-1, -1, 0), Wall.Front);
                            var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(-1, 0, 1), Wall.Left, GetIndex(-1, -1, 0), Wall.Front);

                            int rotation = -blockDefinition.GetTextureRotation(Wall.Bottom, _manager, globalX, globalY, globalZ);

                            var vertXY = new VertexPositionNormalTextureLight(
                                new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 0, -1), _uvOffsets[(6 + rotation) % 4], bottom, AmbientToBrightness(valueXY));
                            var vertY = new VertexPositionNormalTextureLight(
                                new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 0, -1), _uvOffsets[(7 + rotation) % 4], bottom, AmbientToBrightness(valueY));
                            var vertX = new VertexPositionNormalTextureLight(
                                new Vector3(x + 1, y + 0, z + 0), new Vector3(0, 0, -1), _uvOffsets[(5 + rotation) % 4], bottom, AmbientToBrightness(valueX));
                            var vert = new VertexPositionNormalTextureLight(
                                new Vector3(x + 0, y + 0, z + 0), new Vector3(0, 0, -1), _uvOffsets[(4 + rotation) % 4], bottom, AmbientToBrightness(value));

                            if (value + valueXY <= valueY + valueX)
                            {
                                _vertices.Add(vertY);
                                _vertices.Add(vert);
                                _vertices.Add(vertXY);
                                _vertices.Add(vertX);
                            }
                            else
                            {
                                _vertices.Add(vertXY);
                                _vertices.Add(vertY);
                                _vertices.Add(vertX);
                                _vertices.Add(vert);
                            }
                        }


                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsSolidWall(Wall.Front) && southBlock != block))
                        {
                            var front = (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ));
                            var rotation = -blockDefinition.GetTextureRotation(Wall.Front, _manager, globalX, globalY, globalZ);

                            var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(0, 1, -1), Wall.Right, GetIndex(-1, 1, 0), Wall.Front);
                            var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(0, 1, 1), Wall.Left, GetIndex(-1, 1, 0), Wall.Front);
                            var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(0, 1, -1), Wall.Right, GetIndex(1, 1, 0), Wall.Back);
                            var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 1, 0), Wall.Left, GetIndex(0, 1, 1), Wall.Back);

                            var vertY = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 1, 0), _uvOffsets[(6 + rotation) % 4], front, AmbientToBrightness(valueY));
                            var vertXY = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 1, 0), _uvOffsets[(7 + rotation) % 4], front, AmbientToBrightness(valueXY));
                            var vertYZ = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 1, 0), _uvOffsets[(5 + rotation) % 4], front, AmbientToBrightness(valueYZ));
                            var vertXYZ = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 1, 0), _uvOffsets[(4 + rotation) % 4], front, AmbientToBrightness(valueXYZ));

                            if (valueY + valueXYZ >= valueYZ + valueXY)
                            {
                                _vertices.Add(vertY);
                                _vertices.Add(vertXY);
                                _vertices.Add(vertYZ);
                                _vertices.Add(vertXYZ);
                            }
                            else
                            {
                                _vertices.Add(vertXY);
                                _vertices.Add(vertXYZ);
                                _vertices.Add(vertY);
                                _vertices.Add(vertYZ);
                            }
                        }

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSolidWall(Wall.Back) && northBlock != block))
                        {
                            var back = (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ));
                            var rotation = -blockDefinition.GetTextureRotation(Wall.Back, _manager, globalX, globalY, globalZ);
                            var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(-1, -1, 0), Wall.Front);
                            var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(0, -1, 1), Wall.Left, GetIndex(-1, -1, 0), Wall.Front);
                            var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(1, -1, 0), Wall.Back);
                            var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(1, -1, 0), Wall.Left, GetIndex(0, -1, 1), Wall.Back);

                            var vertZ = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 1), new Vector3(0, -1, 0), _uvOffsets[(4 + rotation) % 4], back, AmbientToBrightness(valueZ));
                            var vertXZ = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 1), new Vector3(0, -1, 0), _uvOffsets[(5 + rotation) % 4], back, AmbientToBrightness(valueXZ));
                            var vert = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 0), new Vector3(0, -1, 0), _uvOffsets[(7 + rotation) % 4], back, AmbientToBrightness(value));
                            var vertX = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 0), new Vector3(0, -1, 0), _uvOffsets[(6 + rotation) % 4], back, AmbientToBrightness(valueX));

                            if (value + valueXZ <= valueZ + valueX)
                            {
                                _vertices.Add(vertZ);
                                _vertices.Add(vertXZ);
                                _vertices.Add(vert);
                                _vertices.Add(vertX);
                            }
                            else
                            {
                                _vertices.Add(vertXZ);
                                _vertices.Add(vertX);
                                _vertices.Add(vertZ);
                                _vertices.Add(vert);
                            }
                        }

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsSolidWall(Wall.Right) && westBlock != block))
                        {
                            var left = (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ));
                            var rotation = -blockDefinition.GetTextureRotation(Wall.Left, _manager, globalX, globalY, globalZ);
                            var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(0, 1, -1), Wall.Left, GetIndex(-1, 0, -1), Wall.Front);
                            var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(1, 0, -1), Wall.Left, GetIndex(0, 1, -1), Wall.Back);
                            var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(-1, 0, -1), Wall.Front);
                            var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(0, -1, -1), Wall.Right, GetIndex(1, 0, -1), Wall.Back);

                            var vertY = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 0), new Vector3(-1, 0, 0), _uvOffsets[(7 + rotation) % 4], left, AmbientToBrightness(valueY));
                            var vertYZ = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 1, z + 1), new Vector3(-1, 0, 0), _uvOffsets[(4 + rotation) % 4], left, AmbientToBrightness(valueYZ));
                            var vert = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 0), new Vector3(-1, 0, 0), _uvOffsets[(6 + rotation) % 4], left, AmbientToBrightness(value));
                            var vertZ = new VertexPositionNormalTextureLight(new Vector3(x + 0, y + 0, z + 1), new Vector3(-1, 0, 0), _uvOffsets[(5 + rotation) % 4], left, AmbientToBrightness(valueZ));

                            if (value + valueYZ <= valueZ + valueY)
                            {
                                _vertices.Add(vertY);
                                _vertices.Add(vertYZ);
                                _vertices.Add(vert);
                                _vertices.Add(vertZ);
                            }
                            else
                            {
                                _vertices.Add(vertYZ);
                                _vertices.Add(vertZ);
                                _vertices.Add(vertY);
                                _vertices.Add(vert);
                            }
                        }

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsSolidWall(Wall.Left) && eastBlock != block))
                        {
                            var right = (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ));
                            var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(0, 1, 1), Wall.Left, GetIndex(-1, 0, 1), Wall.Front);
                            var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 0, 1), Wall.Left, GetIndex(0, 1, 1), Wall.Back);
                            var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(0, -1, 1), Wall.Right, GetIndex(-1, 0, 1), Wall.Front);
                            var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(0, -1, 1), Wall.Right, GetIndex(1, 0, 1), Wall.Back);

                            var rotation = -blockDefinition.GetTextureRotation(Wall.Right, _manager, globalX, globalY, globalZ);

                            var vertXYZ = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 1), new Vector3(1, 0, 0), _uvOffsets[(5 + rotation) % 4], right, AmbientToBrightness(valueXYZ));
                            var vertXY = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 1, z + 0), new Vector3(1, 0, 0), _uvOffsets[(6 + rotation) % 4], right, AmbientToBrightness(valueXY));
                            var vertXZ = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 1), new Vector3(1, 0, 0), _uvOffsets[(4 + rotation) % 4], right, AmbientToBrightness(valueXZ));
                            var vertX = new VertexPositionNormalTextureLight(new Vector3(x + 1, y + 0, z + 0), new Vector3(1, 0, 0), _uvOffsets[(7 + rotation) % 4], right, AmbientToBrightness(valueX));

                            if (valueX + valueXYZ >= valueXZ + valueXY)
                            {
                                _vertices.Add(vertXYZ);
                                _vertices.Add(vertXY);
                                _vertices.Add(vertXZ);
                                _vertices.Add(vertX);
                            }
                            else
                            {
                                _vertices.Add(vertXY);
                                _vertices.Add(vertX);
                                _vertices.Add(vertXYZ);
                                _vertices.Add(vertXZ);
                            }
                        }
                    }
                }
            }

            _vertexCount = _vertices.Count;
            _indexCount = _vertices.Count * 6 / 4;

            if (_vertexCount > 0)
            {
                Dispatch(() =>
                {
                    if (_vb == null || ib == null)
                    {
                        _vb = new VertexBuffer(_graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration, _vertexCount);
                    }

                    if (_vertexCount > _vb.VertexCount)
                        _vb.Resize(_vertexCount);


                    _vb.SetData(_vertices.ToArray());
                });
            }

            lock (this)
            {
                if (chunk != null && chunk.Index != ChunkPosition)
                    return _loaded;

                _loaded = true;
                NeedsUpdate |= chunk != this._chunk;
                return !NeedsUpdate;
            }
        }

        private static int VertexAO(int side1, int side2, int corner) => ((side1 & side2) ^ 1) * (3 - (side1 + side2 + corner));

        private uint AmbientToBrightness(uint ambient) => (0xFFFFFF / 2) + (0xFFFFFF / 6 * ambient);

        private static uint VertexAO(IBlockDefinition[] blockDefinitions, int cornerIndex, int side1Index, Wall side1Wall, int side2Index, Wall side2Wall)
        {
            var cornerBlock = blockDefinitions[cornerIndex]?.SolidWall ?? 0;
            var side1Def = blockDefinitions[side1Index];
            var side2Def = blockDefinitions[side2Index];
            var side1 = IsSolidWall(side1Wall, side1Def?.SolidWall ?? 0);
            var side2 = IsSolidWall(side2Wall, side2Def?.SolidWall ?? 0);

            return (uint) VertexAO(side1, side2, cornerBlock == 0 ? 0 : 1);
        }

        private static int GetIndex(int zOffset, int yOffset, int xOffset) => ((((zOffset + 1) * 3) + yOffset + 1) * 3) + xOffset + 1;

        static int IsSolidWall(Wall wall, uint solidWall) => ((int) solidWall >> (int) wall) & 1;

        public void Dispose()
        {
            if (_vb != null)
            {
                _vb.Dispose();
                _vb = null;
            }

            if (_chunk != null)
            {
                _chunk.Changed -= OnChunkChanged;
                _chunk = null;
            }
        }

        private void Dispatch(Action action)
        {
            if (DispatchRequired)
                _dispatcher.Invoke(action);
            else
                action();
        }
    }
}