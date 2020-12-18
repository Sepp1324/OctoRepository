using OctoAwesome.Client.Controls;
using System.Collections.Generic;
using engenious.Graphics;
using System.Linq;
using engenious;
using System;
using System.Windows.Threading;
using System.Threading;
using OctoAwesome.Threading;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private static IndexBuffer ib;

        private readonly SceneControl _sceneControl;
        private readonly Dispatcher dispatcher;
        private Index3? _chunkPosition;
        private ILocalChunkCache _manager;

        /// <summary>
        /// Referenz auf den aktuellen Chunk (falls vorhanden)
        /// </summary>
        private IChunk chunk;

        private readonly IDefinitionManager definitionManager;
        private readonly GraphicsDevice graphicsDevice;
        private readonly object ibLock = new object();
        private int indexCount;
        private bool loaded = false;

        public bool NeedsUpdate = false;
        private readonly Effect simple;

        private readonly Texture2DArray textures;

        private VertexBuffer vb;
        private int vertexCount;

        public ChunkRenderer(SceneControl sceneControl, IDefinitionManager definitionManager, Effect simpleShader,
            GraphicsDevice graphicsDevice, Matrix projection, Texture2DArray textures)
        {
            _sceneControl = sceneControl;
            this.definitionManager = definitionManager;
            this.graphicsDevice = graphicsDevice;
            this.textures = textures;
            dispatcher = Dispatcher.CurrentDispatcher;
            simple = simpleShader;
            GenerateIndexBuffer();
        }

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

        public bool DispatchRequired => Thread.CurrentThread.ManagedThreadId != dispatcher.Thread.ManagedThreadId;

        public void Dispose()
        {
            if (vb != null)
            {
                vb.Dispose();
                vb = null;
            }

            if (chunk != null)
            {
                chunk.Changed -= OnChunkChanged;
                chunk = null;
            }
        }

        public void SetChunk(ILocalChunkCache manager, int x, int y, int z)
        {
            var newPosition = new Index3(x, y, z);

            if (_manager == manager && newPosition == ChunkPosition)
            {
                NeedsUpdate = !loaded;
                return;
            }

            _manager = manager;
            ChunkPosition = newPosition;

            if (chunk != null)
            {
                chunk.Changed -= OnChunkChanged;
                chunk = null;
            }

            loaded = false;
            NeedsUpdate = true;
        }


        private void OnChunkChanged(IChunk c)
        {
            NeedsUpdate = true;
            _sceneControl.Enqueue(this);
        }

        public void Draw(Matrix view, Matrix projection, Index3 shift)
        {
            if (!loaded)
                return;

            var worldViewProj = projection * view * Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X,
                shift.Y * Chunk.CHUNKSIZE_Y,
                shift.Z * Chunk.CHUNKSIZE_Z);

            simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            simple.Parameters["BlockTextures"].SetValue(textures);

            simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (vb == null)
                    return;

                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.VertexBuffer = vb;
                graphicsDevice.IndexBuffer = ib;

                foreach (var pass in simple.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, vertexCount, 0, indexCount / 3);
                }
            }
        }

        public void GenerateIndexBuffer()
        {
            lock (ibLock)
            {
                if (ib != null)
                    return;

                ib = new IndexBuffer(graphicsDevice, DrawElementsType.UnsignedInt,
                    Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Z * 6 * 6);
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

        public bool RegenerateVertexBuffer()
        {
            if (!ChunkPosition.HasValue)
                return false;


            // Chunk nachladen
            if (this.chunk == null)
            {
                this.chunk = _manager.GetChunk(ChunkPosition.Value);
                if (this.chunk == null)
                {
                    //Thread.Sleep(10);
                    //RegenerateVertexBuffer();
                    //NeedsUpdate = false;
                    return false;
                }

                this.chunk.Changed += OnChunkChanged;
            }

            var chunk = this.chunk;
            var vertices = new List<VertexPositionNormalTextureLight>();
            var textureColumns = textures.Width / SceneControl.TEXTURESIZE;
            var textureWidth = 1f / textureColumns;
            var texelSize = 1f / SceneControl.TEXTURESIZE;
            var textureSizeGap = texelSize;
            var textureGap = texelSize / 2;
            // BlockTypes sammlen
            var textureOffsets = new Dictionary<IBlockDefinition, int>();
            // Dictionary<Type, BlockDefinition> definitionMapping = new Dictionary<Type, BlockDefinition>();
            var definitionIndex = 0;
            foreach (var definition in definitionManager.GetBlockDefinitions())
            {
                var textureCount = definition.Textures.Count();
                textureOffsets.Add(definition, definitionIndex);
                // definitionMapping.Add(definition.GetBlockType(), definition);
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

                        var blockDefinition = (IBlockDefinition) definitionManager.GetDefinitionByIndex(block);

                        if (blockDefinition == null)
                            continue;

                        int textureIndex;
                        if (!textureOffsets.TryGetValue(blockDefinition, out textureIndex))
                            continue;

                        // Textur-Koordinate "berechnen"
                        var textureOffset = new Vector2();
                        //Vector2 textureSize = new Vector2(textureWidth - textureSizeGap, textureWidth - textureSizeGap);


                        var topBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));
                        var topBlockDefintion = (IBlockDefinition) definitionManager.GetDefinitionByIndex(topBlock);

                        var globalX = x + chunk.Index.X * Chunk.CHUNKSIZE_X;
                        var globalY = y + chunk.Index.Y * Chunk.CHUNKSIZE_Y;
                        var globalZ = z + chunk.Index.Z * Chunk.CHUNKSIZE_Z;

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsSolidWall(Wall.Bottom) && topBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)) %
                                  textureColumns) * textureWidth) + textureGap,
                                (((textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX, globalY, globalZ)) /
                                  textureColumns) * textureWidth) + textureGap);

                            var rotation =
                                -blockDefinition.GetTextureRotation(Wall.Top, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, 0, 1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Top, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                        }

                        var bottomBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));
                        var bottomBlockDefintion =
                            (IBlockDefinition) definitionManager.GetDefinitionByIndex(bottomBlock);


                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsSolidWall(Wall.Top) && bottomBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX,
                                    globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager, globalX,
                                    globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);

                            var rotation =
                                -blockDefinition.GetTextureRotation(Wall.Bottom, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, 0, -1),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Bottom, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                        }

                        var southBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));
                        var southBlockDefintion = (IBlockDefinition) definitionManager.GetDefinitionByIndex(southBlock);

                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsSolidWall(Wall.Front) && southBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX,
                                    globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager, globalX,
                                    globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);

                            var rotation =
                                -blockDefinition.GetTextureRotation(Wall.Front, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(0, 1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Front, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                        }

                        var northBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));
                        var northBlockDefintion = (IBlockDefinition) definitionManager.GetDefinitionByIndex(northBlock);

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSolidWall(Wall.Back) && northBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ)) %
                                  textureColumns) * textureWidth) + textureGap,
                                (((textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX, globalY, globalZ) /
                                   textureColumns) * textureWidth) + textureGap));

                            var rotation =
                                -blockDefinition.GetTextureRotation(Wall.Back, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(0, -1, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Back, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                        }

                        var westBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));
                        var westBlockDefintion = (IBlockDefinition) definitionManager.GetDefinitionByIndex(westBlock);

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsSolidWall(Wall.Right) && westBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)) %
                                  textureColumns) * textureWidth) + textureGap,
                                (((textureIndex +
                                   blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX, globalY, globalZ)) /
                                  textureColumns) * textureWidth) + textureGap);


                            var rotation =
                                -blockDefinition.GetTextureRotation(Wall.Left, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 1, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 0),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 0, y + 0, z + 1),
                                    new Vector3(-1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Left, _manager, globalX,
                                        globalY, globalZ)),
                                    0));
                        }

                        var eastBlock =
                            _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));
                        var eastBlockDefintion = (IBlockDefinition) definitionManager.GetDefinitionByIndex(eastBlock);

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsSolidWall(Wall.Left) && eastBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX,
                                    globalY, globalZ)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager, globalX,
                                    globalY, globalZ)) / textureColumns) * textureWidth) + textureGap);


                            var rotation =
                                -blockDefinition.GetTextureRotation(Wall.Right, _manager, globalX, globalY, globalZ);

                            var localOffset = vertices.Count;
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(5 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 1, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(6 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 1),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(4 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                            vertices.Add(
                                new VertexPositionNormalTextureLight(
                                    new Vector3(x + 1, y + 0, z + 0),
                                    new Vector3(1, 0, 0),
                                    uvOffsets[(7 + rotation) % 4],
                                    (byte) (textureIndex + blockDefinition.GetTextureIndex(Wall.Right, _manager,
                                        globalX, globalY, globalZ)),
                                    0));
                        }
                    }
                }
            }

            vertexCount = vertices.Count;
            indexCount = vertices.Count * 6 / 4;

            if (vertexCount > 0)
            {
                Dispatch(() =>
                {
                    if (vb == null || ib == null)
                    {
                        vb = new VertexBuffer(graphicsDevice, VertexPositionNormalTextureLight.VertexDeclaration,
                            vertexCount + 2);
                    }

                    if (vertexCount + 2 > vb.VertexCount)
                        vb.Resize(vertexCount + 2);


                    vb.SetData(vertices.ToArray());
                });
            }

            lock (this)
            {
                if (chunk != null && chunk.Index != ChunkPosition)
                {
                    return loaded;
                }

                loaded = true;
                NeedsUpdate |= chunk != this.chunk;
                return !NeedsUpdate;
            }
        }

        private void Dispatch(Action action)
        {
            if (DispatchRequired)
                dispatcher.Invoke(action);
            else
                action();
        }
    }
}