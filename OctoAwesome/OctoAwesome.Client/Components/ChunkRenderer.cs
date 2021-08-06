using System;
using System.Collections.Generic;
using System.Linq;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using OctoAwesome.Client.Controls;
using OctoAwesome.Definitions;
using OctoAwesome.Runtime;
using OctoAwesome.Threading;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private static readonly Vector2[] uvOffsets;
        private static readonly RasterizerState _wireFrameState;

        private readonly SceneControl __sceneControl;

        private readonly IBlockDefinition[] _blockDefinitions;
        private readonly IChunk[] _chunks;
        private readonly LockSemaphore _semaphore = new(1, 1);
        private Index3 _cameraPos;

        /// <summary>
        ///     Referenz auf den aktuellen Chunk (falls vorhanden)
        /// </summary>
        private IChunk _centerChunk;

        private Index3? _chunkPosition;
        private readonly IDefinitionManager _definitionManager;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly object _ibLock = new();

        private int _indexCount;
        private ILocalChunkCache _manager;
        private IPlanet _planet;
        private Index3 _shift;

        private readonly Effect _simple;
        private readonly Dictionary<IBlockDefinition, int> _textureOffsets;

        private readonly Texture2DArray _textures;
        private float _textureSizeGap;
        private readonly List<VertexPositionNormalTextureLight> _vertices;
        private DatabaseProvider dbProvier;

        public bool NeedsUpdate;


        static ChunkRenderer()
        {
            _wireFrameState = new RasterizerState {FillMode = PolygonMode.Line, CullMode = CullMode.CounterClockwise};
            OverrideLightLevel = 0;
            WireFrame = false;
            uvOffsets = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
        }

        public ChunkRenderer(SceneControl sceneControl, IDefinitionManager definitionManager, Effect simpleShader,
            GraphicsDevice graphicsDevice, Matrix projection, Texture2DArray textures)
        {
            __sceneControl = sceneControl;
            _definitionManager = definitionManager;
            _graphicsDevice = graphicsDevice;
            _textures = textures;
            _simple = simpleShader;
            GenerateIndexBuffer();

            _vertices = new List<VertexPositionNormalTextureLight>();
            var textureColumns = textures.Width / SceneControl.TEXTURESIZE;
            _textureSizeGap = 1f / SceneControl.TEXTURESIZE;

            // BlockTypes sammlen
            var localBlockDefinitions = definitionManager.BlockDefinitions;
            _textureOffsets = new Dictionary<IBlockDefinition, int>(localBlockDefinitions.Length);
            var definitionIndex = 0;

            foreach (var definition in localBlockDefinitions)
            {
                var textureCount = definition.Textures.Count();
                _textureOffsets.Add(definition, definitionIndex);
                definitionIndex += textureCount;
            }

            _chunks = new IChunk[27];
            _blockDefinitions = new IBlockDefinition[27];
        }

        public VertexBuffer VertexBuffer { get; private set; }
        public static IndexBuffer IndexBuffer { get; private set; }
        public static float OverrideLightLevel { get; set; }
        public static bool WireFrame { get; set; }

        public bool Loaded { get; set; }

        public int VertexCount { get; private set; }

        /// <summary>
        ///     Adresse des aktuellen Chunks
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

        public void Dispose()
        {
            //CacheCurrentChunkVerticesData();
            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
                VertexBuffer = null;
            }

            if (_centerChunk != null)
            {
                _centerChunk.Changed -= OnChunkChanged;
                _centerChunk = null;
            }
        }

        public Index3 GetShift(Index3 chunkOffset, IPlanet planet)
        {
            if (chunkOffset == _cameraPos)
                return _shift;

            _shift = chunkOffset.ShortestDistanceXY(
                _chunkPosition.Value, new Index2(
                    planet.Size.X,
                    planet.Size.Y));
            _cameraPos = chunkOffset;
            return _shift;
        }

        public void SetChunk(ILocalChunkCache manager, Index3? newPosition, IPlanet planet)
        {
            if (_manager == manager && newPosition == ChunkPosition)
            {
                NeedsUpdate = !Loaded;
                return;
            }

            _manager = manager;
            ChunkPosition = newPosition;

            if (_centerChunk != null)
            {
                _centerChunk.Changed -= OnChunkChanged;
                _centerChunk = null;
            }

            _planet = planet;

            Loaded = false;
            NeedsUpdate = true;
        }


        private void OnChunkChanged(IChunk c)
        {
            NeedsUpdate = true;
            __sceneControl.Enqueue(this);
        }

        public void Draw(Matrix viewProj, Index3 shift)
        {
            if (!Loaded)
                return;

            var worldViewProj = viewProj * Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X,
                shift.Y * Chunk.CHUNKSIZE_Y,
                shift.Z * Chunk.CHUNKSIZE_Z);

            _simple.Parameters["OverrideLightLevel"].SetValue(OverrideLightLevel);
            _simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            _simple.Parameters["BlockTextures"].SetValue(_textures);
            _simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            _simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (VertexBuffer == null)
                    return;

                _graphicsDevice.RasterizerState = WireFrame ? _wireFrameState : RasterizerState.CullCounterClockwise;
                _graphicsDevice.VertexBuffer = VertexBuffer;

                foreach (var pass in _simple.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, VertexCount, 0,
                        _indexCount / 3);
                }
            }
        }

        public void GenerateIndexBuffer()
        {
            lock (_ibLock)
            {
                if (IndexBuffer != null)
                    return;

                IndexBuffer = new IndexBuffer(_graphicsDevice, DrawElementsType.UnsignedInt,
                    Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Z * 6 * 6);
                var indices = new List<int>(IndexBuffer.IndexCount);
                for (var i = 0; i < IndexBuffer.IndexCount * 2 / 3; i += 4)
                {
                    indices.Add(i + 0);
                    indices.Add(i + 1);
                    indices.Add(i + 3);

                    indices.Add(i + 0);
                    indices.Add(i + 3);
                    indices.Add(i + 2);
                }

                IndexBuffer.SetData(indices.ToArray());
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
            using (_semaphore.Wait())
            {
                var chunk = _centerChunk;
                // Chunk nachladen
                if (chunk == null)
                {
                    chunk = _manager.GetChunk(ChunkPosition.Value);

                    if (chunk == null)
                        return false;

                    _centerChunk = chunk;
                    _centerChunk.Changed += OnChunkChanged;
                }

                _vertices.Clear();

                var chunkPos = ChunkPosition.Value;
                for (var x = -1; x < 2; x++)
                for (var y = -1; y < 2; y++)
                for (var z = -1; z < 2; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                    {
                        _chunks[13] = chunk;
                        continue;
                    }

                    var chunkOffset = new Index3(x + chunkPos.X, y + chunkPos.Y, chunkPos.Z + z);
                    _chunks[GetIndex(z, y, x)] = _manager.GetChunk(chunkOffset);
                }

                var offsetVertices = 0;

                for (var z = Chunk.CHUNKSIZE_Z - 1; z >= 0; z -= Chunk.CHUNKSIZE.Z - 1)
                for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    GenerateVertices(chunk, _chunks, uvOffsets, x, y, z, chunkPos, _blockDefinitions, true);

                for (var z = 0; z < Chunk.CHUNKSIZE_Z; z++)
                for (var y = Chunk.CHUNKSIZE_Y - 1; y >= 0; y -= Chunk.CHUNKSIZE.Y - 1)
                for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    GenerateVertices(chunk, _chunks, uvOffsets, x, y, z, chunkPos, _blockDefinitions, true);

                for (var z = 0; z < Chunk.CHUNKSIZE_Z; z++)
                for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                for (var x = Chunk.CHUNKSIZE_X - 1; x >= 0; x -= Chunk.CHUNKSIZE.X - 1)
                    GenerateVertices(chunk, _chunks, uvOffsets, x, y, z, chunkPos, _blockDefinitions, true);

                for (var z = 1; z < Chunk.CHUNKSIZE_Z - 1; z++)
                for (var y = 1; y < Chunk.CHUNKSIZE_Y - 1; y++)
                for (var x = 1; x < Chunk.CHUNKSIZE_X - 1; x++)
                    GenerateVertices(chunk, _chunks, uvOffsets, x, y, z, chunkPos, _blockDefinitions, true);

                return RegisterNewVertices(chunk);
            }
        }
#if DEBUG
        private bool RegisterNewVertices(IChunk chunk)
#else
        private unsafe bool RegisterNewVertices(IChunk chunk)
#endif
        {
            VertexCount = _vertices.Count;
            _indexCount = _vertices.Count * 6 / 4;

            if (VertexCount > 0)
                ThreadingHelper.OnUiThread(t =>
                {
                    if (VertexBuffer == null || IndexBuffer == null)
                        VertexBuffer = new VertexBuffer(_graphicsDevice,
                            VertexPositionNormalTextureLight.VertexDeclaration, VertexCount);

                    if (VertexCount > VertexBuffer.VertexCount)
                        VertexBuffer.Resize(VertexCount);

                    VertexBuffer.SetData(_vertices.ToArray());
                }, null);

            lock (this)
            {
                if (chunk != null && chunk.Index != ChunkPosition) return Loaded;

                Loaded = true;
                NeedsUpdate |= chunk != _centerChunk;
                return !NeedsUpdate;
            }
        }
#if DEBUG
        private void GenerateVertices(IChunk centerChunk, IChunk[] chunks, Vector2[] uvOffsets, int z, int y, int x,
            Index3 chunkPosition, IBlockDefinition[] blockDefinitions, bool getFromManager)
#else
        private unsafe void GenerateVertices(IChunk chunk, IChunk[] chunks, int x, int y, int z, IBlockDefinition[] blockDefinitions, bool getFromManager)
#endif
        {
            var block = centerChunk.GetBlock(x, y, z);

            if (block == 0)
                return;

            var blockDefinition = _definitionManager.GetBlockDefinitionByIndex(block);

            if (blockDefinition == null)
                return;

            if (!_textureOffsets.TryGetValue(blockDefinition, out var textureIndex))
                return;

            if (_vertices.Count == 0)
                _vertices.Capacity = 4096;

#if DEBUG
            var blocks = new ushort[27];
#else
            var blocks = stackalloc ushort[27];
#endif
            ushort topBlock, bottomBlock, southBlock, northBlock, westBlock, eastBlock;
            if (getFromManager)
            {
                IChunk chunk;
                for (var zOffset = -1; zOffset <= 1; zOffset++)
                for (var yOffset = -1; yOffset <= 1; yOffset++)
                for (var xOffset = -1; xOffset <= 1; xOffset++)
                {
                    chunk = chunks[
                        GetIndex(IsBorder(z) * OutsiteOfChunkBorderInDirection(z, zOffset),
                            IsBorder(y) * OutsiteOfChunkBorderInDirection(y, yOffset),
                            IsBorder(x) * OutsiteOfChunkBorderInDirection(x, xOffset))];
                    blocks[GetIndex(zOffset, yOffset, xOffset)] = _manager.GetBlock(
                        ChunkPosition.Value * Chunk.CHUNKSIZE + new Index3(x + xOffset, y + yOffset, z + zOffset));
                }
            }
            else
            {
                for (var zOffset = -1; zOffset <= 1; zOffset++)
                for (var yOffset = -1; yOffset <= 1; yOffset++)
                for (var xOffset = -1; xOffset <= 1; xOffset++)
                    blocks[GetIndex(zOffset, yOffset, xOffset)] = centerChunk.Blocks[
                        Chunk.GetFlatIndex(ChunkPosition.Value * Chunk.CHUNKSIZE +
                                           new Index3(x + xOffset, y + yOffset, z + zOffset))];
            }


            topBlock = blocks[(2 * 3 + 1) * 3 + 1];
            bottomBlock = blocks[(0 * 3 + 1) * 3 + 1];
            southBlock = blocks[(1 * 3 + 2) * 3 + 1];
            northBlock = blocks[(1 * 3 + 0) * 3 + 1];
            westBlock = blocks[(1 * 3 + 1) * 3 + 0];
            eastBlock = blocks[(1 * 3 + 1) * 3 + 2];

            for (var zOffset = -1; zOffset <= 1; zOffset++)
            for (var yOffset = -1; yOffset <= 1; yOffset++)
            for (var xOffset = -1; xOffset <= 1; xOffset++)
                blockDefinitions[GetIndex(zOffset, yOffset, xOffset)] =
                    _definitionManager.GetBlockDefinitionByIndex(blocks[GetIndex(zOffset, yOffset, xOffset)]);

            var topBlockDefintion = blockDefinitions[(2 * 3 + 1) * 3 + 1];
            var bottomBlockDefintion = blockDefinitions[(0 * 3 + 1) * 3 + 1];
            var southBlockDefintion = blockDefinitions[(1 * 3 + 2) * 3 + 1];
            var northBlockDefintion = blockDefinitions[(1 * 3 + 0) * 3 + 1];
            var westBlockDefintion = blockDefinitions[(1 * 3 + 1) * 3 + 0];
            var eastBlockDefintion = blockDefinitions[(1 * 3 + 1) * 3 + 2];
            var globalX = x + centerChunk.Index.X * Chunk.CHUNKSIZE_X;
            var globalY = y + centerChunk.Index.Y * Chunk.CHUNKSIZE_Y;
            var globalZ = z + centerChunk.Index.Z * Chunk.CHUNKSIZE_Z;

            // Top
            if (topBlock == 0 || !topBlockDefintion.IsSolidWall(Wall.Bottom) && topBlock != block)
            {
                var top = (byte) (textureIndex +
                                  blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ));
                var rotation = -blockDefinition.GetTextureRotation(Wall.Top, _manager, globalX, globalY, globalZ);

                var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(1, 0, -1), Wall.Left,
                    GetIndex(1, 1, 0), Wall.Front);
                var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 0, 1), Wall.Left,
                    GetIndex(1, 1, 0), Wall.Front);
                var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(1, 0, -1), Wall.Left,
                    GetIndex(1, -1, 0), Wall.Front);
                var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(1, 0, 1), Wall.Left,
                    GetIndex(1, -1, 0), Wall.Front);

                var vertYZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 1, z + 1),
                    new Vector3(0, 0, 1),
                    uvOffsets[(6 + rotation) % 4],
                    top,
                    AmbientToBrightness(valueYZ));
                var vertXYZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(0, 0, 1),
                    uvOffsets[(7 + rotation) % 4],
                    top,
                    AmbientToBrightness(valueXYZ));
                var vertZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 0, z + 1),
                    new Vector3(0, 0, 1),
                    uvOffsets[(5 + rotation) % 4],
                    top,
                    AmbientToBrightness(valueZ));
                var vertXZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 0, z + 1),
                    new Vector3(0, 0, 1),
                    uvOffsets[(4 + rotation) % 4],
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
            if (bottomBlock == 0 || !bottomBlockDefintion.IsSolidWall(Wall.Top) && bottomBlock != block)
            {
                var bottom = (byte) (textureIndex +
                                     blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ));
                var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(-1, 0, -1), Wall.Left,
                    GetIndex(-1, 1, 0), Wall.Front);
                var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(-1, 0, 1), Wall.Left,
                    GetIndex(-1, 1, 0), Wall.Front);
                var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(-1, 0, -1), Wall.Left,
                    GetIndex(-1, -1, 0), Wall.Front);
                var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(-1, 0, 1), Wall.Left,
                    GetIndex(-1, -1, 0), Wall.Front);

                var rotation = -blockDefinition.GetTextureRotation(Wall.Bottom, _manager, globalX, globalY, globalZ);

                var vertXY = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 0, -1), uvOffsets[(6 + rotation) % 4], bottom,
                    AmbientToBrightness(valueXY));
                var vertY = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 0, -1), uvOffsets[(7 + rotation) % 4], bottom,
                    AmbientToBrightness(valueY));
                var vertX = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 0, z + 0), new Vector3(0, 0, -1), uvOffsets[(5 + rotation) % 4], bottom,
                    AmbientToBrightness(valueX));
                var vert = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 0, z + 0), new Vector3(0, 0, -1), uvOffsets[(4 + rotation) % 4], bottom,
                    AmbientToBrightness(value));

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
            if (southBlock == 0 || !southBlockDefintion.IsSolidWall(Wall.Front) && southBlock != block)
            {
                var front = (byte) (textureIndex +
                                    blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ));
                var rotation = -blockDefinition.GetTextureRotation(Wall.Front, _manager, globalX, globalY, globalZ);

                var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(0, 1, -1), Wall.Right,
                    GetIndex(-1, 1, 0), Wall.Front);
                var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(0, 1, 1), Wall.Left,
                    GetIndex(-1, 1, 0), Wall.Front);
                var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(0, 1, -1), Wall.Right,
                    GetIndex(1, 1, 0), Wall.Back);
                var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 1, 0), Wall.Left,
                    GetIndex(0, 1, 1), Wall.Back);

                var vertY = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 1), uvOffsets[(6 + rotation) % 4], front,
                    AmbientToBrightness(valueY));
                var vertXY = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 1), uvOffsets[(7 + rotation) % 4], front,
                    AmbientToBrightness(valueXY));
                var vertYZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 1), uvOffsets[(5 + rotation) % 4], front,
                    AmbientToBrightness(valueYZ));
                var vertXYZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 1), uvOffsets[(4 + rotation) % 4], front,
                    AmbientToBrightness(valueXYZ));


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
            if (northBlock == 0 || !northBlockDefintion.IsSolidWall(Wall.Back) && northBlock != block)
            {
                var back = (byte) (textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ));
                var rotation = -blockDefinition.GetTextureRotation(Wall.Back, _manager, globalX, globalY, globalZ);
                var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(0, -1, -1), Wall.Right,
                    GetIndex(-1, -1, 0), Wall.Front);
                var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(0, -1, 1), Wall.Left,
                    GetIndex(-1, -1, 0), Wall.Front);
                var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(0, -1, -1), Wall.Right,
                    GetIndex(1, -1, 0), Wall.Back);
                var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(1, -1, 0), Wall.Left,
                    GetIndex(0, -1, 1), Wall.Back);

                var vertZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 0, z + 1), new Vector3(0, -1), uvOffsets[(4 + rotation) % 4], back,
                    AmbientToBrightness(valueZ));
                var vertXZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 0, z + 1), new Vector3(0, -1), uvOffsets[(5 + rotation) % 4], back,
                    AmbientToBrightness(valueXZ));
                var vert = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 0, z + 0), new Vector3(0, -1), uvOffsets[(7 + rotation) % 4], back,
                    AmbientToBrightness(value));
                var vertX = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 0, z + 0), new Vector3(0, -1), uvOffsets[(6 + rotation) % 4], back,
                    AmbientToBrightness(valueX));

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
            if (westBlock == 0 || !westBlockDefintion.IsSolidWall(Wall.Right) && westBlock != block)
            {
                var left = (byte) (textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ));
                var rotation = -blockDefinition.GetTextureRotation(Wall.Left, _manager, globalX, globalY, globalZ);

                var valueY = VertexAO(blockDefinitions, GetIndex(-1, 1, -1), GetIndex(0, 1, -1), Wall.Left,
                    GetIndex(-1, 0, -1), Wall.Front);
                var valueYZ = VertexAO(blockDefinitions, GetIndex(1, 1, -1), GetIndex(1, 0, -1), Wall.Left,
                    GetIndex(0, 1, -1), Wall.Back);
                var value = VertexAO(blockDefinitions, GetIndex(-1, -1, -1), GetIndex(0, -1, -1), Wall.Right,
                    GetIndex(-1, 0, -1), Wall.Front);
                var valueZ = VertexAO(blockDefinitions, GetIndex(1, -1, -1), GetIndex(0, -1, -1), Wall.Right,
                    GetIndex(1, 0, -1), Wall.Back);

                var vertY = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 1, z + 0), new Vector3(-1, 0), uvOffsets[(7 + rotation) % 4], left,
                    AmbientToBrightness(valueY));
                var vertYZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 1, z + 1), new Vector3(-1, 0), uvOffsets[(4 + rotation) % 4], left,
                    AmbientToBrightness(valueYZ));
                var vert = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 0, z + 0), new Vector3(-1, 0), uvOffsets[(6 + rotation) % 4], left,
                    AmbientToBrightness(value));
                var vertZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 0, y + 0, z + 1), new Vector3(-1, 0), uvOffsets[(5 + rotation) % 4], left,
                    AmbientToBrightness(valueZ));

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
            if (eastBlock == 0 || !eastBlockDefintion.IsSolidWall(Wall.Left) && eastBlock != block)
            {
                var right = (byte) (textureIndex +
                                    blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ));
                var valueXY = VertexAO(blockDefinitions, GetIndex(-1, 1, 1), GetIndex(0, 1, 1), Wall.Left,
                    GetIndex(-1, 0, 1), Wall.Front);
                var valueXYZ = VertexAO(blockDefinitions, GetIndex(1, 1, 1), GetIndex(1, 0, 1), Wall.Left,
                    GetIndex(0, 1, 1), Wall.Back);
                var valueX = VertexAO(blockDefinitions, GetIndex(-1, -1, 1), GetIndex(0, -1, 1), Wall.Right,
                    GetIndex(-1, 0, 1), Wall.Front);
                var valueXZ = VertexAO(blockDefinitions, GetIndex(1, -1, 1), GetIndex(0, -1, 1), Wall.Right,
                    GetIndex(1, 0, 1), Wall.Back);

                var rotation = -blockDefinition.GetTextureRotation(Wall.Right, _manager, globalX, globalY, globalZ);

                var vertXYZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 1, z + 1), new Vector3(1, 0), uvOffsets[(5 + rotation) % 4], right,
                    AmbientToBrightness(valueXYZ));
                var vertXY = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 1, z + 0), new Vector3(1, 0), uvOffsets[(6 + rotation) % 4], right,
                    AmbientToBrightness(valueXY));
                var vertXZ = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 0, z + 1), new Vector3(1, 0), uvOffsets[(4 + rotation) % 4], right,
                    AmbientToBrightness(valueXZ));
                var vertX = new VertexPositionNormalTextureLight(
                    new Vector3(x + 1, y + 0, z + 0), new Vector3(1, 0), uvOffsets[(7 + rotation) % 4], right,
                    AmbientToBrightness(valueX));

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

        private static int VertexAO(int side1, int side2, int corner)
        {
            return ((side1 & side2) ^ 1) * (3 - (side1 + side2 + corner));
        }

        private uint AmbientToBrightness(uint ambient)
        {
            return 0xFFFFFF / 2 + 0xFFFFFF / 6 * ambient;
        }

        private static /*unsafe */ uint VertexAO(IBlockDefinition[] blockDefinitions, int cornerIndex, int side1Index,
            Wall side1Wall, int side2Index, Wall side2Wall)
        {
            var cornerBlock = blockDefinitions[cornerIndex]?.SolidWall ?? 0;
            var side1Def = blockDefinitions[side1Index];
            var side2Def = blockDefinitions[side2Index];
            var side1 = IsSolidWall(side1Wall, side1Def?.SolidWall ?? 0);
            var side2 = IsSolidWall(side2Wall, side2Def?.SolidWall ?? 0);

            return (uint) VertexAO(side1, side2, cornerBlock == 0 ? 0 : 1);
        }

        private static int GetIndex(int zOffset, int yOffset, int xOffset)
        {
            return ((zOffset + 1) * 3 + yOffset + 1) * 3 + xOffset + 1;
        }

        private static int IsSolidWall(Wall wall, uint solidWall)
        {
            return ((int) solidWall >> (int) wall) & 1;
        }

        /// <summary>
        /// </summary>
        /// <param name="z"></param>
        /// <returns>0 when not border, 0 becomes -1 and 15 becomes 1</returns>
        private static int IsBorder(int z)
        {
            var tmp = (uint) (z % (Chunk.CHUNKSIZE_X - 1));
            return (int) ~((tmp | (~tmp + 1)) >> 31) & 1;
        }

        /// <summary>
        ///     Determines wether the position is inside the chunk borders or outsite and if its in the neagtiv or positiv
        ///     direction outside the chunk
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static int OutsiteOfChunkBorderInDirection(int pos, int offset)
        {
            return ((pos + offset) >> 31) + (((pos + offset) >> Chunk.LimitX) & ~((pos + offset) >> 31));
        }

        private void Dispatch(Action action)
        {
            action();
        }
    }
}