using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using OctoAwesome.Client.Components;
using System.Drawing.Imaging;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using engenious.UI;
using OctoAwesome.Definitions;

namespace OctoAwesome.Client.Controls
{
    internal sealed class SceneControl : Control, IDisposable
    {
        public static int VIEWRANGE = 4; // Anzahl Chunks als Potenz (Volle Sichtweite)
        public const int TEXTURESIZE = 64;
        public static int Mask;
        public static int Span;
        public static int SpanOver2;

<<<<<<< HEAD
        private PlayerComponent _player;
        private CameraComponent _camera;
        private AssetComponent _assets;
        private Components.EntityComponent _entities;

        private ChunkRenderer[,] _chunkRenderer;
        private List<ChunkRenderer> _orderedChunkRenderer;

        private IPlanet _planet;

        // private List<Index3> distances = new List<Index3>();

        private BasicEffect _sunEffect;
        private BasicEffect _selectionEffect;
        private Matrix _miniMapProjectionMatrix;

        //private Texture2D blockTextures;
        private Texture2DArray _blockTextures;
        private Texture2D _sunTexture;

        private IndexBuffer _selectionIndexBuffer;
        private VertexBuffer _selectionLines;
        private VertexBuffer _billboardVertexbuffer;
        //private VertexPositionColor[] selectionLines;
        //private VertexPositionTexture[] billboardVertices;

        private Index2 _currentChunk = new Index2(-1, -1);

        private Thread _backgroundThread;
        private Thread _backgroundThread2;
        private ILocalChunkCache _localChunkCache;
        private Effect _simpleShader;

        private Thread[] _additionalRegenerationThreads;

        public RenderTarget2D MiniMapTexture { get; set; }
        public RenderTarget2D ControlTexture { get; set; }

        private float sunPosition = 0f;

        public event EventHandler OnCenterChanged;

=======
        private PlayerComponent player;
        private CameraComponent camera;
        private AssetComponent assets;
        private Components.EntityComponent entities;

        private ChunkRenderer[,] chunkRenderer;
        private List<ChunkRenderer> orderedChunkRenderer;

        private IPlanet planet;

        // private List<Index3> distances = new List<Index3>();

        private BasicEffect sunEffect;
        private BasicEffect selectionEffect;
        private Matrix miniMapProjectionMatrix;

        //private Texture2D blockTextures;
        private Texture2DArray blockTextures;
        private Texture2D sunTexture;

        private IndexBuffer selectionIndexBuffer;
        private VertexBuffer selectionLines;
        private VertexBuffer billboardVertexbuffer;
        //private VertexPositionColor[] selectionLines;
        //private VertexPositionTexture[] billboardVertices;

        private Index2 currentChunk = new Index2(-1, -1);

        private Thread backgroundThread;
        private Thread backgroundThread2;
        private ILocalChunkCache localChunkCache;
        private Effect simpleShader;

        private Thread[] _additionalRegenerationThreads;

        public RenderTarget2D MiniMapTexture { get; set; }
        public RenderTarget2D ControlTexture { get; set; }

        private float sunPosition = 0f;

        public event EventHandler OnCenterChanged;

>>>>>>> feature/performance
        private readonly VertexPositionColor[] selectionVertices =
        {
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, -0.001f), Color.Black * 0.5f),
        };
        private readonly VertexPositionTexture[] billboardVertices =
        {
                new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
        };
        private readonly ushort[] selectionIndices =
        {
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
        };

        private ScreenComponent Manager { get; set; }

        private int _fillIncrement;

        public SceneControl(ScreenComponent manager, string style = "") :
            base(manager, style)
        {
            Mask = (int)Math.Pow(2, VIEWRANGE) - 1;
            Span = (int)Math.Pow(2, VIEWRANGE);
            SpanOver2 = Span >> 1;

            player = manager.Player;
            camera = manager.Camera;
            assets = manager.Game.Assets;
            entities = manager.Game.Entity;
            Manager = manager;

            simpleShader = manager.Game.Content.Load<Effect>("simple");
            sunTexture = assets.LoadTexture(typeof(ScreenComponent), "sun");

            //List<Bitmap> bitmaps = new List<Bitmap>();
<<<<<<< HEAD
            var definitions = Manager.Game.DefinitionManager.BlockDefinitions;
=======
            var definitions = Manager.Game.DefinitionManager.GetBlockDefinitions();
>>>>>>> feature/performance
            int textureCount = 0;
            foreach (var definition in definitions)
            {
                textureCount += definition.Textures.Length;
            }
            int bitmapSize = 128;
<<<<<<< HEAD
            _blockTextures = new Texture2DArray(manager.GraphicsDevice, 1, bitmapSize, bitmapSize, textureCount);
=======
            blockTextures = new Texture2DArray(manager.GraphicsDevice, 1, bitmapSize, bitmapSize, textureCount);
>>>>>>> feature/performance
            int layer = 0;
            foreach (var definition in definitions)
            {
                foreach (var bitmap in definition.Textures)
                {
                    System.Drawing.Bitmap texture = manager.Game.Assets.LoadBitmap(definition.GetType(), bitmap);

                    var scaled = texture;//new Bitmap(bitmap, new System.Drawing.Size(bitmapSize, bitmapSize));
                    int[] data = new int[scaled.Width * scaled.Height];
                    var bitmapData = scaled.LockBits(new System.Drawing.Rectangle(0, 0, scaled.Width, scaled.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, data.Length);
                    blockTextures.SetData(data, layer);
                    scaled.UnlockBits(bitmapData);
                    layer++;
                }
            }

            planet = Manager.Game.ResourceManager.GetPlanet(player.Position.Position.Planet);

            // TODO: evtl. Cache-Size (Dimensions) VIEWRANGE + 1

            int range = ((int)Math.Pow(2, VIEWRANGE) - 2) / 2;
<<<<<<< HEAD
            _localChunkCache = new LocalChunkCache(_planet.GlobalChunkCache, VIEWRANGE, range);
=======
            localChunkCache = new LocalChunkCache(planet.GlobalChunkCache, VIEWRANGE, range);
>>>>>>> feature/performance

            chunkRenderer = new ChunkRenderer[
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE),
                planet.Size.Z];
            orderedChunkRenderer = new List<ChunkRenderer>(
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE) * planet.Size.Z);

<<<<<<< HEAD
            for (int i = 0; i < _chunkRenderer.GetLength(0); i++)
            {
                for (int j = 0; j < _chunkRenderer.GetLength(1); j++)
                {
                    ChunkRenderer renderer = new ChunkRenderer(this, Manager.Game.DefinitionManager, _simpleShader, manager.GraphicsDevice, _camera.Projection, _blockTextures);
                    _chunkRenderer[i, j] = renderer;
                    _orderedChunkRenderer.Add(renderer);
=======
            for (int i = 0; i < chunkRenderer.GetLength(0); i++)
            {
                for (int j = 0; j < chunkRenderer.GetLength(1); j++)
                {
                    ChunkRenderer renderer = new ChunkRenderer(this, Manager.Game.DefinitionManager, simpleShader, manager.GraphicsDevice, camera.Projection, blockTextures);
                    chunkRenderer[i, j] = renderer;
                    orderedChunkRenderer.Add(renderer);
>>>>>>> feature/performance
                }
            }

            backgroundThread = new Thread(BackgroundLoop)
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            };
            backgroundThread.Start();

            backgroundThread2 = new Thread(ForceUpdateBackgroundLoop)
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            };
            backgroundThread2.Start();

            int additional;

            if (Environment.ProcessorCount <= 4)
                additional = Environment.ProcessorCount / 3;
            else
                additional = Environment.ProcessorCount - 4;
            additional = additional == 0 ? 1 : additional;
            _fillIncrement = additional + 1;
            additionalFillResetEvents = new AutoResetEvent[additional];
            _additionalRegenerationThreads = new Thread[additional];
            for (int i = 0; i < additional; i++)
            {
                var t = new Thread(AdditionalFillerBackgroundLoop)
                {
                    Priority = ThreadPriority.Lowest,
                    IsBackground = true
                };
                var are = new AutoResetEvent(false);
                t.Start(new object[] { are, i });
                additionalFillResetEvents[i] = are;
                _additionalRegenerationThreads[i] = t;

            }



            var selectionVertices = new[]
            {
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, -0.001f), Color.Black * 0.5f),
            };

            var billboardVertices = new[]
            {
                new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
            };

            var selectionIndices = new short[]
            {
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
            };

            selectionLines = new VertexBuffer(manager.GraphicsDevice, VertexPositionColor.VertexDeclaration, selectionVertices.Length);
            selectionLines.SetData(selectionVertices);

            selectionIndexBuffer = new IndexBuffer(manager.GraphicsDevice, DrawElementsType.UnsignedShort, selectionIndices.Length);
            selectionIndexBuffer.SetData(selectionIndices);

            billboardVertexbuffer = new VertexBuffer(manager.GraphicsDevice, VertexPositionTexture.VertexDeclaration, billboardVertices.Length);
            billboardVertexbuffer.SetData(billboardVertices);


            sunEffect = new BasicEffect(manager.GraphicsDevice)
            {
                TextureEnabled = true
            };

            selectionEffect = new BasicEffect(manager.GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            MiniMapTexture = new RenderTarget2D(manager.GraphicsDevice, 128, 128, PixelInternalFormat.Rgb8); // , false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
<<<<<<< HEAD
            _miniMapProjectionMatrix = Matrix.CreateOrthographic(128, 128, 1, 10000);
=======
            miniMapProjectionMatrix = Matrix.CreateOrthographic(128, 128, 1, 10000);
>>>>>>> feature/performance
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (ControlTexture != null)
                batch.Draw(ControlTexture, contentArea, Color.White * alpha);
        }

        public override void OnResolutionChanged()
        {
            base.OnResolutionChanged();

            if (ControlTexture != null)
            {
                ControlTexture.Dispose();
                ControlTexture = null;
            }

            Manager.Game.Camera.RecreateProjection();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
<<<<<<< HEAD
            if (disposed || _player?.CurrentEntity == null)
=======
            if (disposed || player?.CurrentEntity == null)
>>>>>>> feature/performance
                return;

            sunPosition += (float)gameTime.ElapsedGameTime.TotalMinutes * MathHelper.TwoPi;

<<<<<<< HEAD
            Index3 centerblock = _player.Position.Position.GlobalBlockIndex;
            Index3 renderOffset = _player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;
=======
            Index3 centerblock = player.Position.Position.GlobalBlockIndex;
            Index3 renderOffset = player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;
>>>>>>> feature/performance

            Index3? selected = null;
            Axis? selectedAxis = null;
            Vector3? selectionPoint = null;
            float bestDistance = 9999;
<<<<<<< HEAD
            for (int z = -Player.SelectionRange; z < Player.SelectionRange; z++)
            {
                for (int y = -Player.SelectionRange; y < Player.SelectionRange; y++)
                {
                    for (int x = -Player.SelectionRange; x < Player.SelectionRange; x++)
                    {
                        Index3 range = new Index3(x, y, z);
                        Index3 pos = range + centerblock;
                        ushort block = _localChunkCache.GetBlock(pos);
                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = Manager.Game.DefinitionManager.GetBlockDefinitionByIndex(block);

                        float? distance = Block.Intersect(blockDefinition.GetCollisionBoxes(_localChunkCache, pos.X, pos.Y, pos.Z), pos - renderOffset, _camera.PickRay, out Axis? collisionAxis);
=======
            for (int z = -Player.SELECTIONRANGE; z < Player.SELECTIONRANGE; z++)
            {
                for (int y = -Player.SELECTIONRANGE; y < Player.SELECTIONRANGE; y++)
                {
                    for (int x = -Player.SELECTIONRANGE; x < Player.SELECTIONRANGE; x++)
                    {
                        Index3 range = new Index3(x, y, z);
                        Index3 pos = range + centerblock;
                        ushort block = localChunkCache.GetBlock(pos);
                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = (IBlockDefinition)Manager.Game.DefinitionManager.GetBlockDefinitionByIndex(block);

                        float? distance = Block.Intersect(blockDefinition.GetCollisionBoxes(localChunkCache, pos.X, pos.Y, pos.Z), pos - renderOffset, camera.PickRay, out Axis? collisionAxis);
>>>>>>> feature/performance

                        if (distance.HasValue && distance.Value < bestDistance)
                        {
                            pos.NormalizeXY(planet.Size * Chunk.CHUNKSIZE);
                            selected = pos;
                            selectedAxis = collisionAxis;
                            bestDistance = distance.Value;
                            selectionPoint = (camera.PickRay.Position + (camera.PickRay.Direction * distance)) - (selected - renderOffset);
                        }
                    }
                }
            }

            if (selected.HasValue)
            {
<<<<<<< HEAD
                _player.SelectedBox = selected;
                switch (selectedAxis)
                {
                    case Axis.X: _player.SelectedSide = (_camera.PickRay.Direction.X > 0 ? OrientationFlags.SideWest : OrientationFlags.SideEast); break;
                    case Axis.Y: _player.SelectedSide = (_camera.PickRay.Direction.Y > 0 ? OrientationFlags.SideSouth : OrientationFlags.SideNorth); break;
                    case Axis.Z: _player.SelectedSide = (_camera.PickRay.Direction.Z > 0 ? OrientationFlags.SideBottom : OrientationFlags.SideTop); break;
                }

                _player.SelectedPoint = new Vector2();
                switch (_player.SelectedSide)
                {
                    case OrientationFlags.SideWest:
                        _player.SelectedPoint = new Vector2(1f - selectionPoint.Value.Y, 1f - selectionPoint.Value.Z);
                        _player.SelectedCorner = FindCorner(_player.SelectedPoint.Value, OrientationFlags.Corner011, OrientationFlags.Corner001, OrientationFlags.Corner010, OrientationFlags.Corner000);
                        _player.SelectedEdge = FindEdge(_player.SelectedPoint.Value, OrientationFlags.EdgeWestTop, OrientationFlags.EdgeWestBottom, OrientationFlags.EdgeNorthWest, OrientationFlags.EdgeSouthWest);
                        break;
                    case OrientationFlags.SideEast:
                        _player.SelectedPoint = new Vector2(selectionPoint.Value.Y, 1f - selectionPoint.Value.Z);
                        _player.SelectedCorner = FindCorner(_player.SelectedPoint.Value, OrientationFlags.Corner101, OrientationFlags.Corner111, OrientationFlags.Corner100, OrientationFlags.Corner110);
                        _player.SelectedEdge = FindEdge(_player.SelectedPoint.Value, OrientationFlags.EdgeEastTop, OrientationFlags.EdgeEastBottom, OrientationFlags.EdgeSouthEast, OrientationFlags.EdgeNorthEast);
                        break;
                    case OrientationFlags.SideTop:
                        _player.SelectedPoint = new Vector2(selectionPoint.Value.X, 1f - selectionPoint.Value.Y);
                        _player.SelectedCorner = FindCorner(_player.SelectedPoint.Value, OrientationFlags.Corner011, OrientationFlags.Corner111, OrientationFlags.Corner001, OrientationFlags.Corner101);
                        _player.SelectedEdge = FindEdge(_player.SelectedPoint.Value, OrientationFlags.EdgeNorthTop, OrientationFlags.EdgeSouthTop, OrientationFlags.EdgeWestTop, OrientationFlags.EdgeEastTop);
                        break;
                    case OrientationFlags.SideBottom:
                        _player.SelectedPoint = new Vector2(selectionPoint.Value.X, selectionPoint.Value.Y);
                        _player.SelectedCorner = FindCorner(_player.SelectedPoint.Value, OrientationFlags.Corner000, OrientationFlags.Corner100, OrientationFlags.Corner010, OrientationFlags.Corner110);
                        _player.SelectedEdge = FindEdge(_player.SelectedPoint.Value, OrientationFlags.EdgeSouthBottom, OrientationFlags.EdgeNorthBottom, OrientationFlags.EdgeWestBottom, OrientationFlags.EdgeEastBottom);
                        break;
                    case OrientationFlags.SideNorth:
                        _player.SelectedPoint = new Vector2(1f - selectionPoint.Value.X, 1f - selectionPoint.Value.Z);
                        _player.SelectedCorner = FindCorner(_player.SelectedPoint.Value, OrientationFlags.Corner111, OrientationFlags.Corner011, OrientationFlags.Corner110, OrientationFlags.Corner010);
                        _player.SelectedEdge = FindEdge(_player.SelectedPoint.Value, OrientationFlags.EdgeNorthTop, OrientationFlags.EdgeNorthBottom, OrientationFlags.EdgeNorthEast, OrientationFlags.EdgeNorthWest);
                        break;
                    case OrientationFlags.SideSouth:
                        _player.SelectedPoint = new Vector2(selectionPoint.Value.X, 1f - selectionPoint.Value.Z);
                        _player.SelectedCorner = FindCorner(_player.SelectedPoint.Value, OrientationFlags.Corner001, OrientationFlags.Corner101, OrientationFlags.Corner000, OrientationFlags.Corner100);
                        _player.SelectedEdge = FindEdge(_player.SelectedPoint.Value, OrientationFlags.EdgeSouthTop, OrientationFlags.EdgeSouthBottom, OrientationFlags.EdgeSouthWest, OrientationFlags.EdgeSouthEast);
=======
                player.SelectedBox = selected;
                switch (selectedAxis)
                {
                    case Axis.X: player.SelectedSide = (camera.PickRay.Direction.X > 0 ? OrientationFlags.SideWest : OrientationFlags.SideEast); break;
                    case Axis.Y: player.SelectedSide = (camera.PickRay.Direction.Y > 0 ? OrientationFlags.SideSouth : OrientationFlags.SideNorth); break;
                    case Axis.Z: player.SelectedSide = (camera.PickRay.Direction.Z > 0 ? OrientationFlags.SideBottom : OrientationFlags.SideTop); break;
                }

                player.SelectedPoint = new Vector2();
                switch (player.SelectedSide)
                {
                    case OrientationFlags.SideWest:
                        player.SelectedPoint = new Vector2(1f - selectionPoint.Value.Y, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner011, OrientationFlags.Corner001, OrientationFlags.Corner010, OrientationFlags.Corner000);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeWestTop, OrientationFlags.EdgeWestBottom, OrientationFlags.EdgeNorthWest, OrientationFlags.EdgeSouthWest);
                        break;
                    case OrientationFlags.SideEast:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.Y, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner101, OrientationFlags.Corner111, OrientationFlags.Corner100, OrientationFlags.Corner110);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeEastTop, OrientationFlags.EdgeEastBottom, OrientationFlags.EdgeSouthEast, OrientationFlags.EdgeNorthEast);
                        break;
                    case OrientationFlags.SideTop:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.X, 1f - selectionPoint.Value.Y);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner011, OrientationFlags.Corner111, OrientationFlags.Corner001, OrientationFlags.Corner101);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeNorthTop, OrientationFlags.EdgeSouthTop, OrientationFlags.EdgeWestTop, OrientationFlags.EdgeEastTop);
                        break;
                    case OrientationFlags.SideBottom:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.X, selectionPoint.Value.Y);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner000, OrientationFlags.Corner100, OrientationFlags.Corner010, OrientationFlags.Corner110);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeSouthBottom, OrientationFlags.EdgeNorthBottom, OrientationFlags.EdgeWestBottom, OrientationFlags.EdgeEastBottom);
                        break;
                    case OrientationFlags.SideNorth:
                        player.SelectedPoint = new Vector2(1f - selectionPoint.Value.X, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner111, OrientationFlags.Corner011, OrientationFlags.Corner110, OrientationFlags.Corner010);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeNorthTop, OrientationFlags.EdgeNorthBottom, OrientationFlags.EdgeNorthEast, OrientationFlags.EdgeNorthWest);
                        break;
                    case OrientationFlags.SideSouth:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.X, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner001, OrientationFlags.Corner101, OrientationFlags.Corner000, OrientationFlags.Corner100);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeSouthTop, OrientationFlags.EdgeSouthBottom, OrientationFlags.EdgeSouthWest, OrientationFlags.EdgeSouthEast);
>>>>>>> feature/performance
                        break;
                }

                player.SelectedPoint = new Vector2(
                    Math.Min(1f, Math.Max(0f, player.SelectedPoint.Value.X)),
                    Math.Min(1f, Math.Max(0f, player.SelectedPoint.Value.Y)));
            }
            else
            {
                player.SelectedBox = null;
                player.SelectedPoint = null;
                player.SelectedSide = OrientationFlags.None;
                player.SelectedEdge = OrientationFlags.None;
                player.SelectedCorner = OrientationFlags.None;
            }

<<<<<<< HEAD
            Index2 destinationChunk = new Index2(_player.Position.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != _currentChunk)
=======
            Index2 destinationChunk = new Index2(player.Position.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != currentChunk)
>>>>>>> feature/performance
            {
                fillResetEvent.Set();
            }

            base.OnUpdate(gameTime);
        }

        private AutoResetEvent fillResetEvent = new AutoResetEvent(false);
        private AutoResetEvent[] additionalFillResetEvents;
        private AutoResetEvent forceResetEvent = new AutoResetEvent(false);

        protected override void OnPreDraw(GameTime gameTime)
        {
            if (player?.CurrentEntity == null)
                return;

            if (ControlTexture == null)
                ControlTexture = new RenderTarget2D(Manager.GraphicsDevice, ActualClientArea.Width, ActualClientArea.Height, PixelInternalFormat.Rgb8);


            float octoDaysPerEarthDay = 360f;
            float inclinationVariance = MathHelper.Pi / 3f;

<<<<<<< HEAD
            var playerPosX = _player.Position.Position.GlobalPosition.X / (_planet.Size.X * Chunk.CHUNKSIZE_X) * MathHelper.TwoPi;
            var playerPosY = _player.Position.Position.GlobalPosition.Y / (_planet.Size.Y * Chunk.CHUNKSIZE_Y) * MathHelper.TwoPi;
=======
            float playerPosX = player.Position.Position.GlobalPosition.X / (planet.Size.X * Chunk.CHUNKSIZE_X) * MathHelper.TwoPi;
            float playerPosY = player.Position.Position.GlobalPosition.Y / (planet.Size.Y * Chunk.CHUNKSIZE_Y) * MathHelper.TwoPi;
>>>>>>> feature/performance

            TimeSpan diff = DateTime.UtcNow - new DateTime(1888, 8, 8);

            float inclination = ((float)Math.Sin(playerPosY) * inclinationVariance) + MathHelper.Pi / 6f;
            //Console.WriteLine("Stand: " + (MathHelper.Pi + playerPosX) + " Neigung: " + inclination);
            Matrix sunMovement =
                Matrix.CreateRotationX(inclination) *
                //Matrix.CreateRotationY((((float)gameTime.TotalGameTime.TotalMinutes * MathHelper.TwoPi) + playerPosX) * -1); 
                Matrix.CreateRotationY((float)(MathHelper.TwoPi - ((diff.TotalDays * octoDaysPerEarthDay * MathHelper.TwoPi) % MathHelper.TwoPi)));

            Vector3 sunDirection = Vector3.Transform(new Vector3(0, 0, 1), sunMovement);

            simpleShader.Parameters["DiffuseColor"].SetValue(new Color(190, 190, 190));
            simpleShader.Parameters["DiffuseIntensity"].SetValue(0.6f);
            simpleShader.Parameters["DiffuseDirection"].SetValue(sunDirection);

            // Console.WriteLine(sunDirection);

            // Index3 chunkOffset = player.ActorHost.Position.ChunkIndex;
<<<<<<< HEAD
            Index3 chunkOffset = _camera.CameraChunk;
=======
            Index3 chunkOffset = camera.CameraChunk;
>>>>>>> feature/performance
            Color background = new Color(181, 224, 255);

            Manager.GraphicsDevice.SetRenderTarget(MiniMapTexture);
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Manager.GraphicsDevice.Clear(background);

            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue)
                    continue;

                Index3 shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkPosition.Value, new Index2(
<<<<<<< HEAD
                        _planet.Size.X,
                        _planet.Size.Y));
=======
                        planet.Size.X,
                        planet.Size.Y));
>>>>>>> feature/performance

                BoundingBox chunkBox = new BoundingBox(
                new Vector3(
                    shift.X * Chunk.CHUNKSIZE_X,
                    shift.Y * Chunk.CHUNKSIZE_Y,
                    shift.Z * Chunk.CHUNKSIZE_Z),
                new Vector3(
                    (shift.X + 1) * Chunk.CHUNKSIZE_X,
                    (shift.Y + 1) * Chunk.CHUNKSIZE_Y,
                    (shift.Z + 1) * Chunk.CHUNKSIZE_Z));

                int range = 6;
                if (shift.X >= -range && shift.X <= range &&
                    shift.Y >= -range && shift.Y <= range)
<<<<<<< HEAD
                    renderer.Draw(_camera.MinimapView, _miniMapProjectionMatrix, shift);
=======
                    renderer.Draw(camera.MinimapView, miniMapProjectionMatrix, shift);
>>>>>>> feature/performance
            }

            Manager.GraphicsDevice.SetRenderTarget(ControlTexture);
            Manager.GraphicsDevice.Clear(background);

            Manager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            // Draw Sun
            // GraphicsDevice.RasterizerState = RasterizerState.CullNone;

<<<<<<< HEAD
            if (_camera.View == new Matrix())
                return;

            _sunEffect.Texture = _sunTexture;
            Matrix billboard = Matrix.Invert(_camera.View);
            billboard.Translation = _player.Position.Position.LocalPosition + (sunDirection * -10);
            _sunEffect.World = billboard;
            _sunEffect.View = _camera.View;
            _sunEffect.Projection = _camera.Projection;
            _sunEffect.CurrentTechnique.Passes[0].Apply();
            Manager.GraphicsDevice.VertexBuffer = _billboardVertexbuffer;
=======
            if (camera.View == new Matrix())
                return;

            sunEffect.Texture = sunTexture;
            Matrix billboard = Matrix.Invert(camera.View);
            billboard.Translation = player.Position.Position.LocalPosition + (sunDirection * -10);
            sunEffect.World = billboard;
            sunEffect.View = camera.View;
            sunEffect.Projection = camera.Projection;
            sunEffect.CurrentTechnique.Passes[0].Apply();
            Manager.GraphicsDevice.VertexBuffer = billboardVertexbuffer;
>>>>>>> feature/performance
            Manager.GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, 2);

            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue)
                    continue;

                Index3 shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkPosition.Value, new Index2(
<<<<<<< HEAD
                        _planet.Size.X,
                        _planet.Size.Y));
=======
                        planet.Size.X,
                        planet.Size.Y));
>>>>>>> feature/performance

                BoundingBox chunkBox = new BoundingBox(
                new Vector3(
                    shift.X * Chunk.CHUNKSIZE_X,
                    shift.Y * Chunk.CHUNKSIZE_Y,
                    shift.Z * Chunk.CHUNKSIZE_Z),
                new Vector3(
                    (shift.X + 1) * Chunk.CHUNKSIZE_X,
                    (shift.Y + 1) * Chunk.CHUNKSIZE_Y,
                    (shift.Z + 1) * Chunk.CHUNKSIZE_Z));

<<<<<<< HEAD
                if (_camera.Frustum.Intersects(chunkBox))
                    renderer.Draw(_camera.View, _camera.Projection, shift);
            }



            _entities.Draw(_camera.View, _camera.Projection, chunkOffset, new Index2(_planet.Size.X, _planet.Size.Z));
=======
                if (camera.Frustum.Intersects(chunkBox))
                    renderer.Draw(camera.View, camera.Projection, shift);
            }


>>>>>>> feature/performance

            entities.Draw(camera.View, camera.Projection, chunkOffset, new Index2(planet.Size.X, planet.Size.Z));

            if (player.SelectedBox.HasValue)
            {
                // Index3 offset = player.ActorHost.Position.ChunkIndex * Chunk.CHUNKSIZE;
<<<<<<< HEAD
                Index3 offset = _camera.CameraChunk * Chunk.CHUNKSIZE;
                Index3 planetSize = _planet.Size * Chunk.CHUNKSIZE;
                Index3 relativePosition = new Index3(
                    Index2.ShortestDistanceOnAxis(offset.X, _player.SelectedBox.Value.X, planetSize.X),
                    Index2.ShortestDistanceOnAxis(offset.Y, _player.SelectedBox.Value.Y, planetSize.Y),
                    _player.SelectedBox.Value.Z - offset.Z);

                Vector3 selectedBoxPosition = new Vector3(
                    _player.SelectedBox.Value.X - (chunkOffset.X * Chunk.CHUNKSIZE_X),
                    _player.SelectedBox.Value.Y - (chunkOffset.Y * Chunk.CHUNKSIZE_Y),
                    _player.SelectedBox.Value.Z - (chunkOffset.Z * Chunk.CHUNKSIZE_Z));
                // selectionEffect.World = Matrix.CreateTranslation(selectedBoxPosition);
                _selectionEffect.World = Matrix.CreateTranslation(relativePosition);
                _selectionEffect.View = _camera.View;
                _selectionEffect.Projection = _camera.Projection;
                Manager.GraphicsDevice.VertexBuffer = _selectionLines;
                Manager.GraphicsDevice.IndexBuffer = _selectionIndexBuffer;
                foreach (var pass in _selectionEffect.CurrentTechnique.Passes)
=======
                Index3 offset = camera.CameraChunk * Chunk.CHUNKSIZE;
                Index3 planetSize = planet.Size * Chunk.CHUNKSIZE;
                Index3 relativePosition = new Index3(
                    Index2.ShortestDistanceOnAxis(offset.X, player.SelectedBox.Value.X, planetSize.X),
                    Index2.ShortestDistanceOnAxis(offset.Y, player.SelectedBox.Value.Y, planetSize.Y),
                    player.SelectedBox.Value.Z - offset.Z);

                Vector3 selectedBoxPosition = new Vector3(
                    player.SelectedBox.Value.X - (chunkOffset.X * Chunk.CHUNKSIZE_X),
                    player.SelectedBox.Value.Y - (chunkOffset.Y * Chunk.CHUNKSIZE_Y),
                    player.SelectedBox.Value.Z - (chunkOffset.Z * Chunk.CHUNKSIZE_Z));
                // selectionEffect.World = Matrix.CreateTranslation(selectedBoxPosition);
                selectionEffect.World = Matrix.CreateTranslation(relativePosition);
                selectionEffect.View = camera.View;
                selectionEffect.Projection = camera.Projection;
                Manager.GraphicsDevice.VertexBuffer = selectionLines;
                Manager.GraphicsDevice.IndexBuffer = selectionIndexBuffer;
                foreach (var pass in selectionEffect.CurrentTechnique.Passes)
>>>>>>> feature/performance
                {
                    pass.Apply();
                    Manager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.Lines, 0, 0, 8, 0, 12);
                    //Manager.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.Lines, selectionLines, 0, 8, selectionIndeces, 0, 12);
                }
            }

            Manager.GraphicsDevice.SetRenderTarget(null);
        }


        private void FillChunkRenderer()
        {
            if (player?.CurrentEntity == null)
                return;

<<<<<<< HEAD
            Index2 destinationChunk = new Index2(_player.Position.Position.ChunkIndex);
=======
            Index2 destinationChunk = new Index2(player.Position.Position.ChunkIndex);
>>>>>>> feature/performance

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != currentChunk)
            {
                localChunkCache.SetCenter(
                    new Index2(player.Position.Position.ChunkIndex),
                    b =>
                    {
                        if (b)
                        {
                            fillResetEvent.Set();
                            OnCenterChanged?.Invoke(this, System.EventArgs.Empty);
                        }
                    });

                for (int x = 0; x < Span; x++)
                {
                    for (int y = 0; y < Span; y++)
                    {
                        Index2 local = new Index2(x - SpanOver2, y - SpanOver2) + destinationChunk;
<<<<<<< HEAD
                        local.NormalizeXY(_planet.Size);
=======
                        local.NormalizeXY(planet.Size);
>>>>>>> feature/performance

                        int virtualX = local.X & Mask;
                        int virtualY = local.Y & Mask;

                        int rendererIndex = virtualX +
                            (virtualY << VIEWRANGE);

<<<<<<< HEAD
                        for (int z = 0; z < _planet.Size.Z; z++)
=======
                        for (int z = 0; z < planet.Size.Z; z++)
>>>>>>> feature/performance
                        {
                            chunkRenderer[rendererIndex, z].SetChunk(localChunkCache, local.X, local.Y, z);
                        }
                    }
                }

<<<<<<< HEAD
                Index3 comparationIndex = _player.Position.Position.ChunkIndex;
                _orderedChunkRenderer.Sort((x, y) =>
=======
                Index3 comparationIndex = player.Position.Position.ChunkIndex;
                orderedChunkRenderer.Sort((x, y) =>
>>>>>>> feature/performance
                {
                    if (!x.ChunkPosition.HasValue) return 1;
                    if (!y.ChunkPosition.HasValue) return -1;

<<<<<<< HEAD
                    Index3 distX = comparationIndex.ShortestDistanceXYZ(x.ChunkPosition.Value, _planet.Size);
                    Index3 distY = comparationIndex.ShortestDistanceXYZ(y.ChunkPosition.Value, _planet.Size);
=======
                    Index3 distX = comparationIndex.ShortestDistanceXYZ(x.ChunkPosition.Value, planet.Size);
                    Index3 distY = comparationIndex.ShortestDistanceXYZ(y.ChunkPosition.Value, planet.Size);
>>>>>>> feature/performance
                    return distX.LengthSquared().CompareTo(distY.LengthSquared());
                });

                currentChunk = destinationChunk;
            }

            foreach (var e in additionalFillResetEvents)
                e.Set();

            RegenerateAll(0);
        }

        private void RegenerateAll(int start)
        {
            for (var index = start; index < orderedChunkRenderer.Count; index += _fillIncrement)
            {
<<<<<<< HEAD
                var renderer = _orderedChunkRenderer[index];
=======
                var renderer = orderedChunkRenderer[index];
>>>>>>> feature/performance
                if (renderer.NeedsUpdate)
                {
                    renderer.RegenerateVertexBuffer();
                }
            }
        }

        private void BackgroundLoop()
        {
            while (true)
            {
                fillResetEvent.WaitOne();
                FillChunkRenderer();
            }
        }

        private void AdditionalFillerBackgroundLoop(object oArr)
        {
            var arr = (object[])oArr;
            var are = (AutoResetEvent)arr[0];
            var n = (int)arr[1];
            while (true)
            {
                are.WaitOne();
                RegenerateAll(n + 1);
            }
        }

        private void ForceUpdateBackgroundLoop()
        {
            while (true)
            {
                forceResetEvent.WaitOne();

                while (!forcedRenders.IsEmpty)
                {
                    while (forcedRenders.TryDequeue(out ChunkRenderer r))
                    {
                        r.RegenerateVertexBuffer();
                    }
                }
            }
        }

        #region Converter

        private static OrientationFlags FindEdge(Vector2 point, OrientationFlags upper, OrientationFlags lower, OrientationFlags left, OrientationFlags right)
        {
            if (point.X > point.Y)
            {
                if (1f - point.X > point.Y) return upper;
                else return right;
            }
            else
            {
                if (1f - point.X > point.Y) return left;
                else return lower;
            }
        }

        private static OrientationFlags FindCorner(Vector2 point, OrientationFlags upperLeftCorner, OrientationFlags upperRightCorner, OrientationFlags lowerLeftCorner, OrientationFlags lowerRightCorner)
        {
            if (point.X < 0.5f)
            {
                if (point.Y < 0.5f) return upperLeftCorner;
                else return lowerLeftCorner;
            }
            else
            {
                if (point.Y < 0.5f) return upperRightCorner;
                else return lowerRightCorner;
            }
        }

        #endregion

        private ConcurrentQueue<ChunkRenderer> forcedRenders = new ConcurrentQueue<ChunkRenderer>();
        private bool disposed;

        public void Enqueue(ChunkRenderer chunkRenderer1)
        {
            forcedRenders.Enqueue(chunkRenderer1);
            forceResetEvent.Set();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

<<<<<<< HEAD
            _backgroundThread.Abort();
            _backgroundThread2.Abort();
=======
            backgroundThread.Abort();
            backgroundThread2.Abort();
>>>>>>> feature/performance

            foreach (var thread in _additionalRegenerationThreads)
                thread.Abort();

<<<<<<< HEAD
            foreach (var cr in _chunkRenderer)
                cr.Dispose();

            foreach (var cr in _orderedChunkRenderer)
                cr.Dispose();

            _chunkRenderer = null;
            _orderedChunkRenderer.Clear();

            _localChunkCache = null;

            _selectionIndexBuffer.Dispose();
            _selectionLines.Dispose();
            _billboardVertexbuffer.Dispose();

            _player = null;
            _camera = null;
            _assets = null;
            _entities = null;
            _planet = null;

            _sunEffect.Dispose();
            _selectionEffect.Dispose();

            _blockTextures.Dispose();
            _sunTexture.Dispose();
=======
            foreach (var cr in chunkRenderer)
                cr.Dispose();

            foreach (var cr in orderedChunkRenderer)
                cr.Dispose();

            chunkRenderer = null;
            orderedChunkRenderer.Clear();

            localChunkCache = null;

            selectionIndexBuffer.Dispose();
            selectionLines.Dispose();
            billboardVertexbuffer.Dispose();

            player = null;
            camera = null;
            assets = null;
            entities = null;
            planet = null;

            sunEffect.Dispose();
            selectionEffect.Dispose();

            blockTextures.Dispose();
            sunTexture.Dispose();
>>>>>>> feature/performance
        }
    }
}
