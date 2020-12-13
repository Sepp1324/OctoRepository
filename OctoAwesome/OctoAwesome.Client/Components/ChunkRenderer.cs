using OctoAwesome.Client.Controls;
using System.Collections.Generic;
using engenious.Graphics;
using System.Linq;
using engenious;
using System;
using OctoAwesome.Threading;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private readonly Effect _simple;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly Texture2DArray _textures;
        private readonly LockSemaphore _lockSemaphore;
        //private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Referenz auf den aktuellen Chunk (falls vorhanden)
        /// </summary>
        private IChunk _chunk;
        private bool _loaded = false;

        private VertexBuffer _vb;
        private static IndexBuffer _ib;
        private int _vertexCount;
        private int _indexCount;
        private ILocalChunkCache _manager;

        private readonly SceneControl _sceneControl;
        private readonly IDefinitionManager _definitionManager;

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

        //public bool DispatchRequired => Thread.CurrentThread.ManagedThreadId != _dispatcher.Thread.ManagedThreadId;

        public ChunkRenderer(SceneControl sceneControl, IDefinitionManager definitionManager, Effect simpleShader, GraphicsDevice graphicsDevice, Matrix projection, Texture2DArray textures)
        {
            _sceneControl = sceneControl;
            _definitionManager = definitionManager;
            _graphicsDevice = graphicsDevice;
            _textures = textures;
            _lockSemaphore = new LockSemaphore(1, 1);
            //_dispatcher = Dispatcher.CurrentDispatcher;
            _simple = simpleShader;
            GenerateIndexBuffer();
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

            var worldViewProj = projection * view * Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X,
                shift.Y * Chunk.CHUNKSIZE_Y,
                shift.Z * Chunk.CHUNKSIZE_Z);

            _simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            _simple.Parameters["BlockTextures"].SetValue(_textures);

            _simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            _simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (_vb == null)
                    return;

                _graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                _graphicsDevice.VertexBuffer = _vb;
                _graphicsDevice.IndexBuffer = _ib;

                foreach (var pass in _simple.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, _vertexCount, 0, _indexCount / 3);
                }
            }
        }

        private readonly object _ibLock = new object();
        private Index3? _chunkPosition;

        public void GenerateIndexBuffer()
        {
            lock (_ibLock)
            {
                if (_ib != null)
                    return;

                _ib = new IndexBuffer(_graphicsDevice, DrawElementsType.UnsignedInt, Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Z * 6 * 6);
                var indices = new List<int>(_ib.IndexCount);

                for (var i = 0; i < _ib.IndexCount * 2 / 3; i += 4)
                {
                    indices.Add(i + 0);
                    indices.Add(i + 1);
                    indices.Add(i + 3);

                    indices.Add(i + 0);
                    indices.Add(i + 3);
                    indices.Add(i + 2);
                }
                _ib.SetData(indices.ToArray());
            }
        }

        public bool RegenerateVertexBuffer()
        {
            if (!ChunkPosition.HasValue)
                return false;


            // Chunk nachladen
            if (_chunk == null)
            {
                _chunk = _manager.GetChunk(ChunkPosition.Value);
                if (_chunk == null)
                {
                    return false;
                }

                _chunk.Changed += OnChunkChanged;
            }
            var chunk = this._chunk;
            var vertices = new List<VertexPositionNormalTextureLight>();
            var index = new List<int>();
            var textureColumns = _textures.Width / SceneControl.TEXTURE_SIZE;
            var textureWidth = 1f / textureColumns;
            var texelSize = 1f / SceneControl.TEXTURE_SIZE;
            var textureSizeGap = texelSize;
            var textureGap = texelSize / 2;
            var textureOffsets = new Dictionary<IBlockDefinition, int>();
            var definitionIndex = 0;

            foreach (var definition in _definitionManager.GetBlockDefinitions())
            {
                var textureCount = definition.Textures.Count();
                textureOffsets.Add(definition, definitionIndex);
                definitionIndex += textureCount;
            }

            var uvOffsets = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            for (var z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        var block = chunk.GetBlock(x, y, z);

                        if (block == 0)
                            continue;

                        var blockDefinition = (IBlockDefinition)_definitionManager.GetDefinitionByIndex(block);

                        if (blockDefinition == null)
                            continue;

                        if (!textureOffsets.TryGetValue(blockDefinition, out var textureIndex))
                            continue;

                        // Textur-Koordinate "berechnen"
                        var textureOffset = new Vector2();


                        var topBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));
                        var topBlockDefintion = (IBlockDefinition)_definitionManager.GetDefinitionByIndex(topBlock);

                        var globalX = x + chunk.Index.X * Chunk.CHUNKSIZE_X;
                        var globalY = y + chunk.Index.Y * Chunk.CHUNKSIZE_Y;
                        var globalZ = z + chunk.Index.Z * Chunk.CHUNKSIZE_Z;

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsSolidWall(Wall.Bottom) && topBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);

                            var rotation = -blockDefinition.GetTextureRotation(Wall.Top, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        var bottomBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));
                        var bottomBlockDefintion = (IBlockDefinition)_definitionManager.GetDefinitionByIndex(bottomBlock);


                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsSolidWall(Wall.Top) && bottomBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);

                            var rotation = -blockDefinition.GetTextureRotation(Wall.Bottom, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX, globalY, globalZ)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        var southBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));
                        var southBlockDefintion = (IBlockDefinition)_definitionManager.GetDefinitionByIndex(southBlock);

                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsSolidWall(Wall.Front) && southBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);

                            var rotation = -blockDefinition.GetTextureRotation(Wall.Front, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX, globalY, globalZ)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        var northBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));
                        var northBlockDefintion = (IBlockDefinition)_definitionManager.GetDefinitionByIndex(northBlock);

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSolidWall(Wall.Back) && northBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ) / textureColumns) * textureWidth) + textureGap));

                            var rotation = -blockDefinition.GetTextureRotation(Wall.Back, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        var westBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));
                        var westBlockDefintion = (IBlockDefinition)_definitionManager.GetDefinitionByIndex(westBlock);

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsSolidWall(Wall.Right) && westBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);


                            var rotation = -blockDefinition.GetTextureRotation(Wall.Left, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        var eastBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));
                        var eastBlockDefintion = (IBlockDefinition)_definitionManager.GetDefinitionByIndex(eastBlock);

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsSolidWall(Wall.Left) && eastBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);


                            int rotation = -blockDefinition.GetTextureRotation(Wall.Right, _manager, globalX, globalY, globalZ);

                            int localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte)(textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX, globalY, globalZ)),
                                    0));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }
                    }
                }
            }

            _vertexCount = vertices.Count;
            _indexCount = index.Count;


            if (_vertexCount > 0)
            {
                try
                {
                    Dispatch(() =>
                    {
                        if (_vb == null || _ib == null)
                        {
                            _vb = new VertexBuffer(_graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration, _vertexCount + 2);
                        }
                        if (_vertexCount + 2 > _vb.VertexCount)
                            _vb.Resize(_vertexCount + 2);

                        _vb.SetData(vertices.ToArray());
                    });
                }
                catch (Exception ex)
                {
                    var foo = ex;
                }
            }

            lock (this)
            {
                _loaded = true;
            }

            NeedsUpdate |= chunk != this._chunk;
            return !NeedsUpdate;
        }

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
            //if (DispatchRequired)
            //    _dispatcher.Invoke(action);
            //else
            action();
        }
    }
}
