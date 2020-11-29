using engenious.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using OctoAwesome.Client.Components;
using System.Drawing.Imaging;
using engenious;
using engenious.Graphics;
using engenious.Helper;

namespace OctoAwesome.Client.Controls
{
    internal sealed class SceneControl : Control, IDisposable
    {
        public static int VIEWRANGE = 4; // Anzahl Chunks als Potenz (Volle Sichtweite)
        public const int TEXTURESIZE = 64;
        public static int Mask;
        public static int Span;
        public static int SpanOver2;

        private PlayerComponent _player;
        private CameraComponent _camera;
        private AssetComponent _assets;
        private Components.EntityComponent _entities;

        private ChunkRenderer[,] _chunkRenderer;
        private List<ChunkRenderer> _orderedChunkRenderer;

        private IPlanet _planet;

        // private List<Index3> distances = new List<Index3>();

        private BasicEffect _sunEffect;
        private readonly BasicEffect _selectionEffect;
        private readonly Matrix _miniMapProjectionMatrix;

        //private Texture2D blockTextures;
        private readonly Texture2DArray _blockTextures;
        private readonly Texture2D _sunTexture;

        private readonly IndexBuffer _selectionIndexBuffer;
        private readonly VertexBuffer _selectionLines;
        private readonly VertexBuffer _billboardVertexbuffer;
        //private VertexPositionColor[] selectionLines;
        //private VertexPositionTexture[] billboardVertices;

        private Index2 _currentChunk = new Index2(-1, -1);

        private readonly Thread _backgroundThread;
        private readonly Thread _backgroundThread2;
        private ILocalChunkCache _localChunkCache;
        private readonly Effect _simpleShader;

        private readonly Thread[] _additionalRegenerationThreads;

        public RenderTarget2D MiniMapTexture { get; set; }
        public RenderTarget2D ControlTexture { get; set; }

        private float sunPosition = 0f;

        public event EventHandler OnCenterChanged;

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

            _player = manager.Player;
            _camera = manager.Camera;
            _assets = manager.Game.Assets;
            _entities = manager.Game.Entity;
            Manager = manager;

            _simpleShader = manager.Game.Content.Load<Effect>("simple");
            _sunTexture = _assets.LoadTexture(typeof(ScreenComponent), "sun");

            //List<Bitmap> bitmaps = new List<Bitmap>();
            var definitions = Manager.Game.DefinitionManager.GetBlockDefinitions();
            var textureCount = 0;

            foreach (var definition in definitions)
                textureCount += definition.Textures.Length;

            var bitmapSize = 128;

            _blockTextures = new Texture2DArray(manager.GraphicsDevice, 1, bitmapSize, bitmapSize, textureCount);

            var layer = 0;

            foreach (var definition in definitions)
            {
                foreach (var bitmap in definition.Textures)
                {
                    System.Drawing.Bitmap texture = manager.Game.Assets.LoadBitmap(definition.GetType(), bitmap);

                    var scaled = texture;//new Bitmap(bitmap, new System.Drawing.Size(bitmapSize, bitmapSize));
                    int[] data = new int[scaled.Width * scaled.Height];
                    var bitmapData = scaled.LockBits(new System.Drawing.Rectangle(0, 0, scaled.Width, scaled.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, data.Length);
                    _blockTextures.SetData(data, layer);
                    scaled.UnlockBits(bitmapData);
                    layer++;
                }
            }

            _planet = Manager.Game.ResourceManager.GetPlanet(_player.Position.Position.Planet);

            // TODO: evtl. Cache-Size (Dimensions) VIEWRANGE + 1

            var range = ((int)Math.Pow(2, VIEWRANGE) - 2) / 2;

            _localChunkCache = new LocalChunkCache(_planet.GlobalChunkCache, VIEWRANGE, range);

            _chunkRenderer = new ChunkRenderer[
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE),
                _planet.Size.Z];
            _orderedChunkRenderer = new List<ChunkRenderer>(
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE) * _planet.Size.Z);

            for (int i = 0; i < _chunkRenderer.GetLength(0); i++)
            {
                for (int j = 0; j < _chunkRenderer.GetLength(1); j++)
                {
                    ChunkRenderer renderer = new ChunkRenderer(this, Manager.Game.DefinitionManager, _simpleShader, manager.GraphicsDevice, _camera.Projection, _blockTextures);
                    _chunkRenderer[i, j] = renderer;
                    _orderedChunkRenderer.Add(renderer);
                }
            }

            _backgroundThread = new Thread(BackgroundLoop)
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            };
            _backgroundThread.Start();

            _backgroundThread2 = new Thread(ForceUpdateBackgroundLoop)
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            };
            _backgroundThread2.Start();

            var additional = Environment.ProcessorCount / 3;
            additional = additional == 0 ? 1 : additional;
            _fillIncrement = additional + 1;
            _additionalFillResetEvents = new AutoResetEvent[additional];
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
                _additionalFillResetEvents[i] = are;
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

            _selectionLines = new VertexBuffer(manager.GraphicsDevice, VertexPositionColor.VertexDeclaration, selectionVertices.Length);
            _selectionLines.SetData(selectionVertices);

            _selectionIndexBuffer = new IndexBuffer(manager.GraphicsDevice, DrawElementsType.UnsignedShort, selectionIndices.Length);
            _selectionIndexBuffer.SetData(selectionIndices);

            _billboardVertexbuffer = new VertexBuffer(manager.GraphicsDevice, VertexPositionTexture.VertexDeclaration, billboardVertices.Length);
            _billboardVertexbuffer.SetData(billboardVertices);


            _sunEffect = new BasicEffect(manager.GraphicsDevice)
            {
                TextureEnabled = true
            };

            _selectionEffect = new BasicEffect(manager.GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            MiniMapTexture = new RenderTarget2D(manager.GraphicsDevice, 128, 128, PixelInternalFormat.Rgb8); // , false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
            _miniMapProjectionMatrix = Matrix.CreateOrthographic(128, 128, 1, 10000);
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
            if (_disposed || _player.CurrentEntity == null)
                return;

            sunPosition += (float)gameTime.ElapsedGameTime.TotalMinutes * MathHelper.TwoPi;

            var centerBlock = _player.Position.Position.GlobalBlockIndex;
            var renderOffset = _player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;

            Index3? selected = null;
            Axis? selectedAxis = null;
            Vector3? selectionPoint = null;
            float bestDistance = 9999;
            for (var z = -Player.SELECTIONRANGE; z < Player.SELECTIONRANGE; z++)
            {
                for (var y = -Player.SELECTIONRANGE; y < Player.SELECTIONRANGE; y++)
                {
                    for (var x = -Player.SELECTIONRANGE; x < Player.SELECTIONRANGE; x++)
                    {
                        var range = new Index3(x, y, z);
                        var pos = range + centerBlock;
                        var block = _localChunkCache.GetBlock(pos);
                        if (block == 0)
                            continue;

                        var blockDefinition = (IBlockDefinition)Manager.Game.DefinitionManager.GetDefinitionByIndex(block);

                        var distance = Block.Intersect(blockDefinition.GetCollisionBoxes(_localChunkCache, pos.X, pos.Y, pos.Z), pos - renderOffset, _camera.PickRay, out Axis? collisionAxis);

                        if (distance.HasValue && distance.Value < bestDistance)
                        {
                            pos.NormalizeXY(_planet.Size * Chunk.CHUNKSIZE);
                            selected = pos;
                            selectedAxis = collisionAxis;
                            bestDistance = distance.Value;
                            selectionPoint = (_camera.PickRay.Position + (_camera.PickRay.Direction * distance)) - (selected - renderOffset);
                        }
                    }
                }
            }

            if (selected.HasValue)
            {
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
                        break;
                }

                _player.SelectedPoint = new Vector2(
                    Math.Min(1f, Math.Max(0f, _player.SelectedPoint.Value.X)),
                    Math.Min(1f, Math.Max(0f, _player.SelectedPoint.Value.Y)));
            }
            else
            {
                _player.SelectedBox = null;
                _player.SelectedPoint = null;
                _player.SelectedSide = OrientationFlags.None;
                _player.SelectedEdge = OrientationFlags.None;
                _player.SelectedCorner = OrientationFlags.None;
            }

            var destinationChunk = new Index2(_player.Position.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != _currentChunk)
                _fillResetEvent.Set();

            base.OnUpdate(gameTime);
        }

        private readonly AutoResetEvent _fillResetEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent[] _additionalFillResetEvents;
        private readonly AutoResetEvent _forceResetEvent = new AutoResetEvent(false);

        protected override void OnPreDraw(GameTime gameTime)
        {
            if (_player.CurrentEntity == null)
                return;

            if (ControlTexture == null)
                ControlTexture = new RenderTarget2D(Manager.GraphicsDevice, ActualClientArea.Width, ActualClientArea.Height, PixelInternalFormat.Rgb8);


            var octoDaysPerEarthDay = 360f;
            var inclinationVariance = MathHelper.Pi / 3f;

            var playerPosX = _player.Position.Position.GlobalPosition.X / (_planet.Size.X * Chunk.CHUNKSIZE_X) * MathHelper.TwoPi;
            var playerPosY = _player.Position.Position.GlobalPosition.Y / (_planet.Size.Y * Chunk.CHUNKSIZE_Y) * MathHelper.TwoPi;

            var diff = DateTime.UtcNow - new DateTime(1888, 8, 8);

            var inclination = ((float)Math.Sin(playerPosY) * inclinationVariance) + MathHelper.Pi / 6f;
            //Console.WriteLine("Stand: " + (MathHelper.Pi + playerPosX) + " Neigung: " + inclination);
            var sunMovement =
                Matrix.CreateRotationX(inclination) *
                //Matrix.CreateRotationY((((float)gameTime.TotalGameTime.TotalMinutes * MathHelper.TwoPi) + playerPosX) * -1); 
                Matrix.CreateRotationY((float)(MathHelper.TwoPi - ((diff.TotalDays * octoDaysPerEarthDay * MathHelper.TwoPi) % MathHelper.TwoPi)));

            var sunDirection = Vector3.Transform(new Vector3(0, 0, 1), sunMovement);

            _simpleShader.Parameters["DiffuseColor"].SetValue(new Color(190, 190, 190));
            _simpleShader.Parameters["DiffuseIntensity"].SetValue(0.6f);
            _simpleShader.Parameters["DiffuseDirection"].SetValue(sunDirection);

            // Console.WriteLine(sunDirection);

            // Index3 chunkOffset = player.ActorHost.Position.ChunkIndex;
            var chunkOffset = _camera.CameraChunk;
            var background = new Color(181, 224, 255);

            Manager.GraphicsDevice.SetRenderTarget(MiniMapTexture);
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Manager.GraphicsDevice.Clear(background);

            foreach (var renderer in _chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue)
                    continue;

                var shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkPosition.Value, new Index2(
                        _planet.Size.X,
                        _planet.Size.Y));

                var chunkBox = new BoundingBox(
                new Vector3(
                    shift.X * Chunk.CHUNKSIZE_X,
                    shift.Y * Chunk.CHUNKSIZE_Y,
                    shift.Z * Chunk.CHUNKSIZE_Z),
                new Vector3(
                    (shift.X + 1) * Chunk.CHUNKSIZE_X,
                    (shift.Y + 1) * Chunk.CHUNKSIZE_Y,
                    (shift.Z + 1) * Chunk.CHUNKSIZE_Z));

                var range = 3;

                if (shift.X >= -range && shift.X <= range &&
                    shift.Y >= -range && shift.Y <= range)
                    renderer.Draw(_camera.MinimapView, _miniMapProjectionMatrix, shift);
            }

            Manager.GraphicsDevice.SetRenderTarget(ControlTexture);
            Manager.GraphicsDevice.Clear(background);

            Manager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            // Draw Sun
            // GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            if (_camera.View == new Matrix())
                return;

            _sunEffect.Texture = _sunTexture;
            var billboard = Matrix.Invert(_camera.View);
            billboard.Translation = _player.Position.Position.LocalPosition + (sunDirection * -10);
            _sunEffect.World = billboard;
            _sunEffect.View = _camera.View;
            _sunEffect.Projection = _camera.Projection;
            _sunEffect.CurrentTechnique.Passes[0].Apply();
            Manager.GraphicsDevice.VertexBuffer = _billboardVertexbuffer;
            Manager.GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, 2);

            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var renderer in _chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue)
                    continue;

                var shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkPosition.Value, new Index2(
                        _planet.Size.X,
                        _planet.Size.Y));

                var chunkBox = new BoundingBox(
                new Vector3(
                    shift.X * Chunk.CHUNKSIZE_X,
                    shift.Y * Chunk.CHUNKSIZE_Y,
                    shift.Z * Chunk.CHUNKSIZE_Z),
                new Vector3(
                    (shift.X + 1) * Chunk.CHUNKSIZE_X,
                    (shift.Y + 1) * Chunk.CHUNKSIZE_Y,
                    (shift.Z + 1) * Chunk.CHUNKSIZE_Z));

                if (_camera.Frustum.Intersects(chunkBox))
                    renderer.Draw(_camera.View, _camera.Projection, shift);
            }

            _entities.Draw(_camera.View, _camera.Projection, chunkOffset, new Index2(_planet.Size.X, _planet.Size.Z));

            if (_player.SelectedBox.HasValue)
            {
                // Index3 offset = player.ActorHost.Position.ChunkIndex * Chunk.CHUNKSIZE;
                var offset = _camera.CameraChunk * Chunk.CHUNKSIZE;
                var planetSize = _planet.Size * Chunk.CHUNKSIZE;
                var relativePosition = new Index3(
                    Index2.ShortestDistanceOnAxis(offset.X, _player.SelectedBox.Value.X, planetSize.X),
                    Index2.ShortestDistanceOnAxis(offset.Y, _player.SelectedBox.Value.Y, planetSize.Y),
                    _player.SelectedBox.Value.Z - offset.Z);

                var selectedBoxPosition = new Vector3(
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
            if (_player.CurrentEntity == null)
                return;

            var destinationChunk = new Index2(_player.Position.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != _currentChunk)
            {
                _localChunkCache.SetCenter(
                    new Index2(_player.Position.Position.ChunkIndex),
                    b =>
                    {
                        if (b)
                        {
                            _fillResetEvent.Set();
                            OnCenterChanged?.Invoke(this, System.EventArgs.Empty);
                        }
                    });

                for (var x = 0; x < Span; x++)
                {
                    for (var y = 0; y < Span; y++)
                    {
                        var local = new Index2(x - SpanOver2, y - SpanOver2) + destinationChunk;
                        local.NormalizeXY(_planet.Size);

                        var virtualX = local.X & Mask;
                        var virtualY = local.Y & Mask;

                        var rendererIndex = virtualX +
                                            (virtualY << VIEWRANGE);

                        for (var z = 0; z < _planet.Size.Z; z++)
                            _chunkRenderer[rendererIndex, z].SetChunk(_localChunkCache, local.X, local.Y, z);
                    }
                }

                var comparationIndex = _player.Position.Position.ChunkIndex;
                _orderedChunkRenderer.Sort((x, y) =>
                {
                    if (!x.ChunkPosition.HasValue) return 1;
                    if (!y.ChunkPosition.HasValue) return -1;

                    var distX = comparationIndex.ShortestDistanceXYZ(x.ChunkPosition.Value, _planet.Size);
                    var distY = comparationIndex.ShortestDistanceXYZ(y.ChunkPosition.Value, _planet.Size);
                    return distX.LengthSquared().CompareTo(distY.LengthSquared());
                });

                _currentChunk = destinationChunk;
            }

            foreach (var e in _additionalFillResetEvents)
                e.Set();

            RegenerateAll(0);
        }

        private void RegenerateAll(int start)
        {
            for (var index = start; index < _orderedChunkRenderer.Count; index += _fillIncrement)
            {
                var renderer = _orderedChunkRenderer[index];

                if (renderer.NeedsUpdate)
                    renderer.RegenerateVertexBuffer();
            }
        }

        private void BackgroundLoop()
        {
            while (true)
            {
                _fillResetEvent.WaitOne();
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
                _forceResetEvent.WaitOne();

                while (!_forcedRenders.IsEmpty)
                {
                    while (_forcedRenders.TryDequeue(out ChunkRenderer r))
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

        private ConcurrentQueue<ChunkRenderer> _forcedRenders = new ConcurrentQueue<ChunkRenderer>();
        private bool _disposed;

        public void Enqueue(ChunkRenderer chunkRenderer1)
        {
            _forcedRenders.Enqueue(chunkRenderer1);
            _forceResetEvent.Set();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _backgroundThread.Abort();
            _backgroundThread2.Abort();

            foreach (var thread in _additionalRegenerationThreads)
                thread.Abort();

            foreach (var chunkRenderer in _chunkRenderer)
                chunkRenderer.Dispose();

            foreach (var orderedChunkRenderer in _orderedChunkRenderer)
                orderedChunkRenderer.Dispose();

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
        }
    }
}
