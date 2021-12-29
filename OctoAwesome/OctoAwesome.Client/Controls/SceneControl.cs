using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using engenious.UI;
using engenious.UserDefined.Effects;
using OctoAwesome.Client.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;
using EventArgs = System.EventArgs;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace OctoAwesome.Client.Controls
{
    internal sealed class SceneControl : Control, IDisposable
    {
        public const int TEXTURE_SIZE = 64;
        public static int VIEW_RANGE = 4; // Anzahl Chunks als Potenz (Volle Sichtweite)
        private static int Mask;
        private static int Span;
        private static int SpanOver2;
        private readonly AutoResetEvent[] _additionalFillResetEvents;

        private readonly VertexBuffer _billboardVertexBuffer;

        private readonly int _fillIncrement;

        private readonly VertexPositionTexture[] _billboardVertices =
        {
            new(new(-0.5f, 0.5f), new(0, 0)),
            new(new(0.5f, 0.5f), new(1, 0)),
            new(new(-0.5f, -0.5f), new(0, 1)),
            new(new(0.5f, 0.5f), new(1, 0)),
            new(new(0.5f, -0.5f), new(1, 1)),
            new(new(-0.5f, -0.5f), new(0, 1))
        };

        //private Texture2D blockTextures;
        private readonly Texture2DArray _blockTextures;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly AutoResetEvent _fillResetEvent = new(false);

        private readonly ConcurrentQueue<ChunkRenderer> _forcedRenders = new();
        private readonly AutoResetEvent _forceResetEvent = new(false);
        private readonly Matrix _miniMapProjectionMatrix;
        private readonly List<ChunkRenderer> _orderedChunkRenderer;
        private readonly BasicEffect _selectionEffect;

        private readonly IndexBuffer _selectionIndexBuffer;
        private readonly VertexBuffer _selectionLines;

        private readonly VertexPositionColor[] _selectionVertices =
        {
            new(new(-0.001f, +1.001f, +1.001f), Color.Black * 0.5f),
            new(new(+1.001f, +1.001f, +1.001f), Color.Black * 0.5f),
            new(new(-0.001f, -0.001f, +1.001f), Color.Black * 0.5f),
            new(new(+1.001f, -0.001f, +1.001f), Color.Black * 0.5f),
            new(new(-0.001f, +1.001f, -0.001f), Color.Black * 0.5f),
            new(new(+1.001f, +1.001f, -0.001f), Color.Black * 0.5f),
            new(new(-0.001f, -0.001f, -0.001f), Color.Black * 0.5f),
            new(new(+1.001f, -0.001f, -0.001f), Color.Black * 0.5f)
        };

        private readonly chunkEffect _simpleShader;

        private readonly float _sphereRadius;
        private readonly float _sphereRadiusSquared;

        private readonly BasicEffect _sunEffect;
        private readonly Texture2D _sunTexture;
        private AssetComponent _assets;
        private CameraComponent _camera;

        private ChunkRenderer[,] _chunkRenderer;

        private Index2 _currentChunk = new(-1, -1);
        private bool _disposed;
        private EntityGameComponent _entities;
        private ILocalChunkCache _localChunkCache;

        private IPlanet _planet;

        private PlayerComponent _player;

        private float _sunPosition;

        public SceneControl(ScreenComponent manager, string style = "") : base(manager, style)
        {
            Mask = (int)Math.Pow(2, VIEW_RANGE) - 1;
            Span = (int)Math.Pow(2, VIEW_RANGE);
            SpanOver2 = Span >> 1;

            _player = manager.Player;
            _camera = manager.Camera;
            _assets = manager.Game.Assets;
            _entities = manager.Game.Entity;
            Manager = manager;

            _cancellationTokenSource = new();

            var chunkDiag = (float)Math.Sqrt(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_X + Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Y + Chunk.CHUNKSIZE_Z * Chunk.CHUNKSIZE_Z);
            var tmpSphereRadius = (float)(Math.Sqrt(Span * Chunk.CHUNKSIZE_X * Span * Chunk.CHUNKSIZE_X * 3) / 3 + _camera.NearPlaneDistance + chunkDiag / 2);
            _sphereRadius = tmpSphereRadius - chunkDiag / 2;
            _sphereRadiusSquared = tmpSphereRadius * tmpSphereRadius;

            _simpleShader = manager.Game.Content.Load<chunkEffect>("Effects/chunkEffect");
            _sunTexture = _assets.LoadTexture("sun");

            //List<Bitmap> bitmaps = new List<Bitmap>();
            var definitions = Manager.Game.DefinitionManager.BlockDefinitions;
            var textureCount = definitions.Sum(definition => definition.Textures.Length);
            var bitmapSize = 128;
            _blockTextures = new(manager.GraphicsDevice, 1, bitmapSize, bitmapSize, textureCount);
            var layer = 0;
            foreach (var definition in definitions)
            foreach (var bitmap in definition.Textures)
            {
                var texture = manager.Game.Assets.LoadBitmap(definition.GetType(), bitmap);

                var scaled = texture; //new Bitmap(bitmap, new System.Drawing.Size(bitmapSize, bitmapSize));
                var data = new int[scaled.Width * scaled.Height];
                var bitmapData = scaled.LockBits(new(0, 0, scaled.Width, scaled.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(bitmapData.Scan0, data, 0, data.Length);
                _blockTextures.SetData(data, layer);
                scaled.UnlockBits(bitmapData);
                layer++;
            }

            _planet = Manager.Game.ResourceManager.GetPlanet(_player.Position.Position.Planet);

            // TODO: evtl. Cache-Size (Dimensions) VIEW_RANGE + 1

            var range = ((int)Math.Pow(2, VIEW_RANGE) - 2) / 2;
            _localChunkCache = new LocalChunkCache(_planet.GlobalChunkCache, VIEW_RANGE, range);

            _chunkRenderer = new ChunkRenderer[(int)Math.Pow(2, VIEW_RANGE) * (int)Math.Pow(2, VIEW_RANGE), _planet.Size.Z];
            _orderedChunkRenderer = new((int)Math.Pow(2, VIEW_RANGE) * (int)Math.Pow(2, VIEW_RANGE) * _planet.Size.Z);

            for (var i = 0; i < _chunkRenderer.GetLength(0); i++)
            for (var j = 0; j < _chunkRenderer.GetLength(1); j++)
            {
                var renderer = new ChunkRenderer(this, Manager.Game.DefinitionManager, _simpleShader, manager.GraphicsDevice, _camera.Projection, _blockTextures);
                _chunkRenderer[i, j] = renderer;
                _orderedChunkRenderer.Add(renderer);
            }

            var token = _cancellationTokenSource.Token;

            var backgroundTask = new Task(BackgroundLoop, token, token, TaskCreationOptions.LongRunning);
            backgroundTask.Start();

            var backgroundThread2 = new Task(ForceUpdateBackgroundLoop, token, token, TaskCreationOptions.LongRunning);
            backgroundThread2.Start();

            int additional;

            if (Environment.ProcessorCount <= 4)
                additional = Environment.ProcessorCount / 3;
            else
                additional = Environment.ProcessorCount - 4;
            additional = additional == 0 ? 1 : additional;
            _fillIncrement = additional + 1;
            _additionalFillResetEvents = new AutoResetEvent[additional];
            var additionalRegenerationThreads = new Task[additional];

            for (var i = 0; i < additional; i++)
            {
                var are = new AutoResetEvent(false);
                var t = new Task(AdditionalFillerBackgroundLoop, (are, i, token), token, TaskCreationOptions.LongRunning);
                t.Start();
                _additionalFillResetEvents[i] = are;
                additionalRegenerationThreads[i] = t;
            }

            var selectionVertices = new[]
            {
                new VertexPositionColor(new(-0.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new(+1.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new(-0.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new(+1.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new(-0.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new(+1.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new(-0.001f, -0.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new(+1.001f, -0.001f, -0.001f), Color.Black * 0.5f)
            };

            var billboardVertices = new[]
            {
                new VertexPositionTexture(new(-0.5f, 0.5f), new(0, 0)),
                new VertexPositionTexture(new(0.5f, 0.5f), new(1, 0)),
                new VertexPositionTexture(new(-0.5f, -0.5f), new(0, 1)),
                new VertexPositionTexture(new(0.5f, 0.5f), new(1, 0)),
                new VertexPositionTexture(new(0.5f, -0.5f), new(1, 1)),
                new VertexPositionTexture(new(-0.5f, -0.5f), new(0, 1))
            };

            var selectionIndices = new short[]
            {
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
            };

            _selectionLines = new(manager.GraphicsDevice, VertexPositionColor.VertexDeclaration, selectionVertices.Length);
            _selectionLines.SetData(selectionVertices);

            _selectionIndexBuffer = new(manager.GraphicsDevice, DrawElementsType.UnsignedShort, selectionIndices.Length);
            _selectionIndexBuffer.SetData(selectionIndices);

            _billboardVertexBuffer = new(manager.GraphicsDevice, VertexPositionTexture.VertexDeclaration, billboardVertices.Length);
            _billboardVertexBuffer.SetData(billboardVertices);


            _sunEffect = new(manager.GraphicsDevice)
            {
                TextureEnabled = true
            };

            _selectionEffect = new(manager.GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            MiniMapTexture = new(manager.GraphicsDevice, 128, 128, PixelInternalFormat.Rgb8); // , false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
            ShadowMap = new(manager.GraphicsDevice, 8192, 8192, PixelInternalFormat.DepthComponent32);
            ShadowMap.SamplerState = new() { AddressU = TextureWrapMode.ClampToEdge, AddressV = TextureWrapMode.ClampToEdge, TextureCompareMode = TextureCompareMode.CompareRefToTexture, TextureCompareFunction = TextureCompareFunc.LessOrEequal };
            _miniMapProjectionMatrix = Matrix.CreateOrthographic(128, 128, 1, 10000);
        }

        public RenderTarget2D MiniMapTexture { get; set; }

        public RenderTarget2D ControlTexture { get; set; }

        public RenderTarget2D ShadowMap { get; set; }

        private ScreenComponent Manager { get; }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            foreach (var cr in _chunkRenderer)
                cr.Dispose();

            foreach (var cr in _orderedChunkRenderer)
                cr.Dispose();

            foreach (var renderer in _chunkRenderer)
                renderer.SetChunk(null, null, null);

            _chunkRenderer = null;
            _orderedChunkRenderer.Clear();

            _localChunkCache = null;

            _selectionIndexBuffer.Dispose();
            _selectionLines.Dispose();
            _billboardVertexBuffer.Dispose();

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

        public event EventHandler OnCenterChanged;

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
            if (_disposed || _player?.CurrentEntity == null)
                return;

            _sunPosition += (float)gameTime.ElapsedGameTime.TotalMinutes * MathHelper.TwoPi;

            var centerBlock = _player.Position.Position.GlobalBlockIndex;
            var renderOffset = _player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;

            var selBlock = GetSelectedBlock(centerBlock, renderOffset, out var selected, out var selectedAxis, out var selectionPoint);
            var funcBlock = GetSelectedFunctionalBlock(centerBlock, renderOffset, out var selectedFunc, out var selectedFuncAxis, out var selectionFuncPoint);


            //TODO Distance check, so entity doesnt always get selected if a block is in front of it
            if (selectedFunc.HasValue && selectionFuncPoint.HasValue)
            {
                selected = selectedFunc;
                selectionPoint = selectionFuncPoint;
                selectedAxis = selectedFuncAxis;
                _player.Selection = funcBlock;
            }
            else
            {
                _player.Selection = selBlock;
            }

            if (selected.HasValue && selectionPoint.HasValue)
            {
                _player.SelectedBox = selected;
                switch (selectedAxis)
                {
                    case Axis.X:
                        _player.SelectedSide = _camera.PickRay.Direction.X > 0
                            ? OrientationFlags.SideWest
                            : OrientationFlags.SideEast;
                        break;
                    case Axis.Y:
                        _player.SelectedSide = _camera.PickRay.Direction.Y > 0
                            ? OrientationFlags.SideSouth
                            : OrientationFlags.SideNorth;
                        break;
                    case Axis.Z:
                        _player.SelectedSide = _camera.PickRay.Direction.Z > 0
                            ? OrientationFlags.SideBottom
                            : OrientationFlags.SideTop;
                        break;
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

                _player.SelectedPoint = new Vector2(Math.Min(1f, Math.Max(0f, _player.SelectedPoint.Value.X)), Math.Min(1f, Math.Max(0f, _player.SelectedPoint.Value.Y)));
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
            if (destinationChunk != _currentChunk) _fillResetEvent.Set();

            base.OnUpdate(gameTime);
        }

        private BlockInfo GetSelectedBlock(Index3 centerBlock, Index3 renderOffset, out Index3? selected, out Axis? selectedAxis, out Vector3? selectionPoint)
        {
            selected = null;
            selectedAxis = null;
            selectionPoint = null;
            float bestDistance = 9999;
            BlockInfo block = default;
            //var pickEndPost = centerBlock + (camera.PickRay.Position + (camera.PickRay.Direction * Player.SELECTION_RANGE));
            //var pickStartPos = centerBlock + camera.PickRay.Position;
            for (var z = -Player.SELECTION_RANGE; z < Player.SELECTION_RANGE; z++)
            for (var y = -Player.SELECTION_RANGE; y < Player.SELECTION_RANGE; y++)
            for (var x = -Player.SELECTION_RANGE; x < Player.SELECTION_RANGE; x++)
            {
                var range = new Index3(x, y, z);
                var pos = range + centerBlock;
                var localBlock = _localChunkCache.GetBlockInfo(pos);

                if (localBlock.Block == 0)
                    continue;

                var blockDefinition = Manager.Game.DefinitionManager.GetBlockDefinitionByIndex(localBlock.Block);

                var distance = Block.Intersect(blockDefinition.GetCollisionBoxes(_localChunkCache, pos.X, pos.Y, pos.Z), pos - renderOffset, _camera.PickRay, out var collisionAxis);

                if (distance.HasValue && distance.Value < bestDistance)
                {
                    pos.NormalizeXY(_planet.Size * Chunk.CHUNKSIZE);
                    selected = pos;
                    //var futureselected = PythonBresenham((int)pickStartPos.X, (int)pickStartPos.Y, (int)pickStartPos.Z, (int)pickEndPost.X, (int)pickEndPost.Y, (int)pickEndPost.Z);
                    //if (futureselected is not null)
                    //{
                    //    futureselected.Value.NormalizeXY(planet.Size * Chunk.CHUNKSIZE);

                    //    Debug.WriteLine($"Old Selection: {selected}, New Selection: {futureselected}");

                    //    selected = futureselected;
                    //}
                    selectedAxis = collisionAxis;
                    bestDistance = distance.Value;
                    selectionPoint = _camera.PickRay.Position + _camera.PickRay.Direction * distance -
                                     (selected - renderOffset);
                    block = localBlock;
                }
            }

            return block;
        }

        private FunctionalBlock GetSelectedFunctionalBlock(Index3 centerblock, Index3 renderOffset, out Index3? selected, out Axis? selectedAxis, out Vector3? selectionPoint)
        {
            selected = null;
            selectedAxis = null;
            selectionPoint = null;
            float bestDistance = 9999;
            FunctionalBlock functionalBlock = null;

            //Index3 centerBlock = player.Position.Position.GlobalBlockIndex;
            //Index3 renderOffset = player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;
            foreach (var funcBlock in Manager.Game.Simulation.Simulation.FunctionalBlocks)
            {
                if (!funcBlock.ContainsComponent<PositionComponent>() ||
                    !funcBlock.ContainsComponent<BoxCollisionComponent>())
                    continue;

                var posComponent = funcBlock.GetComponent<PositionComponent>();
                var boxCollisionComponent = funcBlock.GetComponent<BoxCollisionComponent>();

                if (posComponent.Position.GlobalPosition.X - Player.SELECTION_RANGE < centerblock.X && posComponent.Position.GlobalPosition.X + Player.SELECTION_RANGE > centerblock.X && posComponent.Position.GlobalPosition.Y - Player.SELECTION_RANGE < centerblock.Y && posComponent.Position.GlobalPosition.Y + Player.SELECTION_RANGE > centerblock.Y && posComponent.Position.GlobalPosition.Z - Player.SELECTION_RANGE < centerblock.Z && posComponent.Position.GlobalPosition.Z + Player.SELECTION_RANGE > centerblock.Z)
                {
                    var distance = Block.Intersect(boxCollisionComponent.BoundingBoxes, posComponent.Position.GlobalBlockIndex - renderOffset, _camera.PickRay, out var collisionAxis);

                    if (distance.HasValue && distance.Value < bestDistance)
                    {
                        posComponent.Position.GlobalBlockIndex.NormalizeXY(_planet.Size * Chunk.CHUNKSIZE);
                        selected = posComponent.Position.GlobalBlockIndex;

                        selectedAxis = collisionAxis;
                        bestDistance = distance.Value;
                        selectionPoint = _camera.PickRay.Position + _camera.PickRay.Direction * distance - (selected - renderOffset);
                        functionalBlock = funcBlock;
                    }
                }
            }

            return functionalBlock;
        }

        protected override void OnPreDraw(GameTime gameTime)
        {
            if (_player?.CurrentEntity == null)
                return;

            ControlTexture ??= new(Manager.GraphicsDevice, ActualClientArea.Width, ActualClientArea.Height, PixelInternalFormat.Rgb8);

            const float octoDaysPerEarthDay = 360f;
            const float inclinationVariance = MathHelper.Pi / 3f;

            var playerPosX = _player.Position.Position.GlobalPosition.X / (_planet.Size.X * Chunk.CHUNKSIZE_X) * MathHelper.TwoPi;
            var playerPosY = _player.Position.Position.GlobalPosition.Y / (_planet.Size.Y * Chunk.CHUNKSIZE_Y) * MathHelper.TwoPi;

            var diff = DateTime.UtcNow - new DateTime(888, 8, 8);

            var inclination = (float)Math.Sin(playerPosY) * inclinationVariance + MathHelper.Pi / 6f;
            var sunMovement =
                Matrix.CreateRotationX(inclination) *
                //Matrix.CreateRotationY((((float)gameTime.TotalGameTime.TotalMinutes * MathHelper.TwoPi) + playerPosX) * -1); 
                Matrix.CreateRotationY((float)(MathHelper.TwoPi - diff.TotalDays * octoDaysPerEarthDay * MathHelper.TwoPi % MathHelper.TwoPi));

            var sunDirection = Vector3.Transform(sunMovement, new(0, 0, 1));

            _simpleShader.Ambient.MainPass.Apply();
            _simpleShader.Ambient.ShadowMap = ShadowMap;
            _simpleShader.Ambient.DiffuseColor = new Color(190, 190, 190);
            _simpleShader.Ambient.DiffuseIntensity = 0.6f;
            _simpleShader.Ambient.DiffuseDirection = sunDirection;

            var chunkOffset = _camera.CameraChunk;
            var background = new Color(181, 224, 255);

            var casters = GetCasters();
            var cropMatrix = CreateCropMatrix(_player.Position.Position.LocalPosition, sunDirection, casters, casters);

            DrawMiniMap(chunkOffset, cropMatrix, background);
            DrawShadowMap(sunDirection, chunkOffset, cropMatrix);
            DrawWorld(gameTime, sunDirection, chunkOffset, background, cropMatrix);
        }

        private void DrawShadowMap(Vector3 sunDirection, Index3 chunkOffset, Matrix cropMatrix)
        {
            Manager.GraphicsDevice.SetRenderTarget(ShadowMap);
            Manager.GraphicsDevice.Clear(ClearBufferMask.DepthBufferBit);

            Manager.GraphicsDevice.BlendState = BlendState.Opaque;
            Manager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            DrawChunksShadow(chunkOffset, cropMatrix);
        }

        private BoundingBox CreateTransformedAABB(BoundingBox boundingBox, ref Matrix transformation)
        {
            Span<Vector4> corners = stackalloc Vector4[8];
            corners[0] = new(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z, 1.0f);
            corners[1] = new(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z, 1.0f);
            corners[2] = new(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z, 1.0f);
            corners[3] = new(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z, 1.0f);
            corners[4] = new(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z, 1.0f);
            corners[5] = new(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z, 1.0f);
            corners[6] = new(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z, 1.0f);
            corners[7] = new(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z, 1.0f);

            Vector3 min = new(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new(float.MinValue, float.MinValue, float.MinValue);

            for (var i = 0; i < 8; i++)
            {
                var transformed = Vector4.Transform(transformation, corners[i]);
                var perspective = new Vector3(transformed.X / transformed.W, transformed.Y / transformed.W, transformed.Z / transformed.W);

                min = Vector3.Min(perspective, min);
                max = Vector3.Max(perspective, max);
            }

            return new(min, max);
        }

        private Matrix BoundingBoxToProjection(BoundingBox cropBoundingBox)
        {
            var scaleX = 2.0f / (cropBoundingBox.Max.X - cropBoundingBox.Min.X);
            var scaleY = 2.0f / (cropBoundingBox.Max.Y - cropBoundingBox.Min.Y);
            var scaleZ = 2.0f / (cropBoundingBox.Max.Z - cropBoundingBox.Min.Z);

            var offsetX = -0.5f * (cropBoundingBox.Max.X + cropBoundingBox.Min.X) * scaleX;
            var offsetY = -0.5f * (cropBoundingBox.Max.Y + cropBoundingBox.Min.Y) * scaleY;
            var offsetZ = -0.5f *(cropBoundingBox.Max.Z + cropBoundingBox.Min.Z) * scaleZ;
            return new(scaleX, 0.0f, 0.0f, 0.0f, 0.0f, scaleY, 0.0f, 0.0f, 0.0f, 0.0f, scaleZ, 0.0f, offsetX, offsetY, offsetZ, 1.0f);
        }

        private BoundingBox BoundingBoxFromFrustum(BoundingFrustum boundingFrustum)
        {
            var viewInvert = Matrix.Invert(boundingFrustum.Matrix);
            return CreateTransformedAABB(new(-Vector3.One, Vector3.One), ref viewInvert);
        }

        private Matrix CreateCropMatrix(Vector3 light, Vector3 lightDir, IEnumerable<BoundingBox> casters, IEnumerable<BoundingBox> receivers)
        {
            var splitFrustum = _camera.Frustum;
            var lightViewProjMatrix = Matrix.CreateOrthographic(ShadowMap.Width, ShadowMap.Height, 0.1f, 100) * Matrix.CreateLookAt(light - lightDir * 500, light + lightDir * 500, Vector3.UnitX);
            BoundingBox receiverBB = default, casterBB = default, splitBB;

            foreach (var caster in casters)
            {
                var bb = CreateTransformedAABB(caster, ref lightViewProjMatrix);
                BoundingBox.CreateMerged(ref casterBB, ref bb, out casterBB);
            }

            foreach (var receiver in receivers)
            {
                var bb = CreateTransformedAABB(receiver, ref lightViewProjMatrix);
                BoundingBox.CreateMerged(ref receiverBB, ref bb, out receiverBB);
            }

            splitBB = CreateTransformedAABB(BoundingBoxFromFrustum(splitFrustum), ref lightViewProjMatrix);

            BoundingBox cropBB = default;

            cropBB.Min.X = Math.Max(Math.Max(casterBB.Min.X, receiverBB.Min.X), splitBB.Min.X);
            cropBB.Max.X = Math.Max(Math.Max(casterBB.Max.X, receiverBB.Max.X), splitBB.Max.X);

            cropBB.Min.Y = Math.Max(Math.Max(casterBB.Min.Y, receiverBB.Min.Y), splitBB.Min.Y);
            cropBB.Max.Y = Math.Max(Math.Max(casterBB.Max.Y, receiverBB.Max.Y), splitBB.Max.Y);

            cropBB.Min.Z = Math.Min(casterBB.Min.Z, splitBB.Min.Z);
            cropBB.Max.Z = Math.Max(receiverBB.Max.Z, splitBB.Max.Z);

            return BoundingBoxToProjection(cropBB) * lightViewProjMatrix;
        }

        private BoundingBox[] GetCasters()
        {
            var chunkOffset = _camera.CameraChunk;
            var casters = new List<BoundingBox>();

            casters.Add(new(new(-4 * Chunk.CHUNKSIZE_X, -4 * Chunk.CHUNKSIZE_Y, -chunkOffset.Z * Chunk.CHUNKSIZE_Z), new(4 * Chunk.CHUNKSIZE_X, 4 * Chunk.CHUNKSIZE_Y, (_planet.Size.Z - chunkOffset.Z) * Chunk.CHUNKSIZE_Z)));

            foreach (var entity in _entities.Entities)
            {
                var p = entity.Components.GetComponent<PositionComponent>();
                var offset = p.Position.ChunkIndex.ShortestDistanceXY(chunkOffset, new Index2(_planet.Size));
                var viewDist = 1 << VIEW_RANGE;

                if (offset.X > viewDist | offset.X < -viewDist || offset.Y > viewDist || offset.Y < -viewDist)
                    continue;

                var pB = entity.Components.GetComponent<BodyComponent>();
                var size = pB == null ? new(1) : new Vector3(pB.Radius, pB.Radius, pB.Height);
                //var offsetBlocks = new Vector3(offset.X * Chunk.CHUNKSIZE_X, offset.Y * Chunk.CHUNKSIZE_Y, offset.Z * Chunk.CHUNKSIZE_Z);

                casters.Add(new(p.Position.LocalPosition - size, p.Position.LocalPosition + size));
            }

            return casters.ToArray();
        }

        private void DrawWorld(GameTime gameTime, Vector3 sunDirection, Index3 chunkOffset, Color background, Matrix cropMatrix)
        {
            Manager.GraphicsDevice.SetRenderTarget(ControlTexture);
            Manager.GraphicsDevice.Clear(background);

            Manager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            if (_camera.View == new Matrix())
                return;

            DrawSun(gameTime, chunkOffset, sunDirection);

            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Manager.GraphicsDevice.IndexBuffer = ChunkRenderer.IndexBuffer;
            var viewProjC = _camera.Projection * _camera.View;

            DrawChunks(chunkOffset, viewProjC, cropMatrix);

            _entities.Draw(gameTime, _camera.View, _camera.Projection, chunkOffset, new(_planet.Size.X, _planet.Size.Z));

            DrawSelectionBox(chunkOffset);

            Manager.GraphicsDevice.Debug.RenderBoundingFrustum(new(cropMatrix), Matrix.CreateTranslation(chunkOffset * new Vector3(Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Chunk.CHUNKSIZE_Z)), _camera.View, _camera.Projection);

            Manager.GraphicsDevice.SetRenderTarget(null!);
        }

        private void DrawSelectionBox(Index3 chunkOffset)
        {
            if (_player.SelectedBox.HasValue)
            {
                // Index3 offset = player.ActorHost.Position.ChunkIndex * Chunk.CHUNKSIZE;
                var offset = _camera.CameraChunk * Chunk.CHUNKSIZE;
                var planetSize = _planet.Size * Chunk.CHUNKSIZE;
                var relativePosition = new Index3(Index2.ShortestDistanceOnAxis(offset.X, _player.SelectedBox.Value.X, planetSize.X), Index2.ShortestDistanceOnAxis(offset.Y, _player.SelectedBox.Value.Y, planetSize.Y), _player.SelectedBox.Value.Z - offset.Z);

                var selectedBoxPosition = new Vector3(_player.SelectedBox.Value.X - chunkOffset.X * Chunk.CHUNKSIZE_X, _player.SelectedBox.Value.Y - chunkOffset.Y * Chunk.CHUNKSIZE_Y, _player.SelectedBox.Value.Z - chunkOffset.Z * Chunk.CHUNKSIZE_Z);
                // selectionEffect.World = Matrix.CreateTranslation(selectedBoxPosition);
                _selectionEffect.World = Matrix.CreateTranslation(relativePosition);
                _selectionEffect.View = _camera.View;
                _selectionEffect.Projection = _camera.Projection;
                Manager.GraphicsDevice.VertexBuffer = _selectionLines;
                Manager.GraphicsDevice.IndexBuffer = _selectionIndexBuffer;

                foreach (var pass in _selectionEffect.CurrentTechnique!.Passes)
                {
                    pass.Apply();
                    Manager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.Lines, 0, 0, 8, 0, 12);
                    //Manager.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.Lines, selectionLines, 0, 8, selectionIndeces, 0, 12);
                }
            }
        }

        private void DrawMiniMap(Index3 chunkOffset, Matrix cropMatrix, Color background)
        {
            Manager.GraphicsDevice.SetRenderTarget(MiniMapTexture);
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Manager.GraphicsDevice.Clear(background);
            Manager.GraphicsDevice.IndexBuffer = ChunkRenderer.IndexBuffer;
            var viewProj = _miniMapProjectionMatrix * _camera.MinimapView;

            foreach (var renderer in _chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue || !renderer.Loaded || !renderer.CanRender)
                    continue;

                var shift = renderer.GetShift(chunkOffset, _planet);

                const int range = 6;
                if (shift.X is >= -range and <= range && shift.Y is >= -range and <= range)
                    renderer.Draw(viewProj, cropMatrix, shift);
            }
        }

        private void DrawSun(GameTime gameTime, Index3 chunkOffset, Vector3 sunDirection)
        {
            Manager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            _sunEffect.CurrentTechnique?.Passes[0].Apply();
            _sunEffect.Texture = _sunTexture;
            var billboard = Matrix.Invert(_camera.View);
            billboard.Translation = _player.Position.Position.LocalPosition + sunDirection * -10;
            _sunEffect.World = billboard;
            _sunEffect.View = _camera.View;
            _sunEffect.Projection = _camera.Projection;

            Manager.GraphicsDevice.VertexBuffer = _billboardVertexBuffer;
            Manager.GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, 6);
        }


        private void DrawChunks(Index3 chunkOffset, Matrix viewProj, Matrix cropMatrix)
        {
            var spherePos = _camera.PickRay.Position + _camera.PickRay.Direction * _sphereRadius;

            foreach (var renderer in _chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue || !renderer.Loaded || !renderer.CanRender)
                    continue;

                var shift = renderer.GetShift(chunkOffset, _planet);

                var chunkPos = new Vector3(shift.X * Chunk.CHUNKSIZE_X + Chunk.CHUNKSIZE_X / 2, shift.Y * Chunk.CHUNKSIZE_Y + Chunk.CHUNKSIZE_Y / 2, shift.Z * Chunk.CHUNKSIZE_Z + Chunk.CHUNKSIZE_Z / 2);

                var frustumDist = spherePos - chunkPos;
                if (frustumDist.LengthSquared < _sphereRadiusSquared)
                    renderer.Draw(viewProj, cropMatrix, shift);
            }
        }

        private void DrawChunksShadow(Index3 chunkOffset, Matrix viewProj)
        {
            var spherePos = _camera.PickRay.Position + _camera.PickRay.Direction * _sphereRadius;

            foreach (var renderer in _chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue || !renderer.Loaded || !renderer.CanRender)
                    continue;

                var shift = renderer.GetShift(chunkOffset, _planet);

                var chunkPos = new Vector3(shift.X * Chunk.CHUNKSIZE_X + Chunk.CHUNKSIZE_X / 2, shift.Y * Chunk.CHUNKSIZE_Y + Chunk.CHUNKSIZE_Y / 2, shift.Z * Chunk.CHUNKSIZE_Z + Chunk.CHUNKSIZE_Z / 2);

                //var frustumDist = spherePos - chunkPos;
                //if (frustumDist.LengthSquared < _sphereRadiusSquared)
                renderer.DrawShadow(viewProj, shift);
            }
        }

        private void FillChunkRenderer()
        {
            if (_player?.CurrentEntity == null)
                return;

            var destinationChunk = new Index2(_player.Position.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != _currentChunk)
            {
                _localChunkCache.SetCenter(
                    new(_player.Position.Position.ChunkIndex),
                    b =>
                    {
                        if (b)
                        {
                            _fillResetEvent.Set();
                            OnCenterChanged?.Invoke(this, EventArgs.Empty);
                        }
                    });

                for (var x = 0; x < Span; x++)
                for (var y = 0; y < Span; y++)
                {
                    var local = new Index2(x - SpanOver2, y - SpanOver2) + destinationChunk;
                    local.NormalizeXY(_planet.Size);

                    var virtualX = local.X & Mask;
                    var virtualY = local.Y & Mask;

                    var rendererIndex = virtualX + (virtualY << VIEW_RANGE);

                    for (var z = 0; z < _planet.Size.Z; z++)
                        _chunkRenderer[rendererIndex, z].SetChunk(_localChunkCache, new Index3(local.X, local.Y, z),
                            _player.Position.Planet);
                }

                var comparisonIndex = _player.Position.Position.ChunkIndex;
                _orderedChunkRenderer.Sort((x, y) =>
                {
                    if (!x.ChunkPosition.HasValue) return 1;
                    if (!y.ChunkPosition.HasValue) return -1;

                    var distX = comparisonIndex.ShortestDistanceXYZ(x.ChunkPosition.Value, _planet.Size);
                    var distY = comparisonIndex.ShortestDistanceXYZ(y.ChunkPosition.Value, _planet.Size);
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
                if (renderer.NeedsUpdate) renderer.RegenerateVertexBuffer();
            }
        }

        private void BackgroundLoop(object state)
        {
            var token = state is CancellationToken stateToken ? stateToken : CancellationToken.None;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                _fillResetEvent.WaitOne();
                FillChunkRenderer();
            }
        }

        private void AdditionalFillerBackgroundLoop(object state)
        {
            var (@event, n, token) = ((AutoResetEvent Event, int N, CancellationToken Token))state;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                @event.WaitOne();
                RegenerateAll(n + 1);
            }
        }

        private void ForceUpdateBackgroundLoop(object state)
        {
            var token = state is CancellationToken stateToken ? stateToken : CancellationToken.None;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                _forceResetEvent.WaitOne();

                while (!_forcedRenders.IsEmpty)
                while (_forcedRenders.TryDequeue(out var r))
                    r.RegenerateVertexBuffer();
            }
        }

        public void Enqueue(ChunkRenderer chunkRenderer1)
        {
            _forcedRenders.Enqueue(chunkRenderer1);
            _forceResetEvent.Set();
        }

        private Index3? CheckPosition(int x, int y, int z)
        {
            var pos = new Index3(x, y, z);
            var block = _localChunkCache.GetBlock(pos);
            return block != 0 ? pos : null;
        }

        private Index3? PythonBresenham(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var dz = Math.Abs(z1 - z0);

            var sx = x0 < x1 ? 1 : -1;
            var sy = y0 < y1 ? 1 : -1;
            var sz = z0 < z1 ? 1 : -1;

            if (dx >= dy && dx >= dz)
            {
                var p1 = 2 * dy - dx;
                var p2 = 2 * dz - dx;

                while (x0 != x1)
                {
                    x0 += sx;
                    if (p1 >= 0)
                    {
                        y0 += sy;
                        p1 -= 2 * dx;
                    }

                    if (p2 >= 0)
                    {
                        z0 += sz;
                        p2 -= 2 * dx;
                    }

                    p1 += 2 * dy;
                    p2 += 2 * dz;

                    var pos = CheckPosition(x0, y0, z0);

                    if (pos.HasValue)
                        return pos;
                }
            }
            else if (dy >= dx && dy >= dz)
            {
                var p1 = 2 * dx - dy;
                var p2 = 2 * dz - dy;
                while (y0 != y1)
                {
                    y0 += sy;
                    if (p1 >= 0)
                    {
                        x0 += sx;
                        p1 -= 2 * dy;
                    }

                    if (p2 >= 0)
                    {
                        z0 += sz;
                        p2 -= 2 * dy;
                    }

                    p1 += 2 * dx;
                    p2 += 2 * dz;

                    var pos = CheckPosition(x0, y0, z0);

                    if (pos.HasValue)
                        return pos;
                }
            }
            else
            {
                var p1 = 2 * dy - dz;
                var p2 = 2 * dx - dz;

                while (z0 != z1)
                {
                    z0 += sz;
                    if (p1 >= 0)
                    {
                        y0 += sy;
                        p1 -= 2 * dz;
                    }

                    if (p2 >= 0)
                    {
                        x0 += sx;
                        p2 -= 2 * dz;
                    }

                    p1 += 2 * dy;
                    p2 += 2 * dx;

                    var pos = CheckPosition(x0, y0, z0);

                    if (pos.HasValue)
                        return pos;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="x0">StartPosition X</param>
        /// <param name="y0">StartPosition Y</param>
        /// <param name="z0">StartPosition Z</param>
        /// <param name="x1">EndPosition X</param>
        /// <param name="y1">EndPosition Y</param>
        /// <param name="z1">EndPosition Z</param>
        private Index3? SimpleBresenham(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int dz = Math.Abs(z1 - z0), sz = z0 < z1 ? 1 : -1;
            int err = dx + dy + dz, e2; /* error value e_xy */

            while (true)
            {
                var pos = new Index3(x0, y0, z0);
                var block = _localChunkCache.GetBlock(pos);
                var isBlock = block != 0;

                if (isBlock)
                    return pos;

                if (x0 == x1 && y0 == y1 && z0 == z1)
                    return null;

                e2 = 2 * err;
                if (e2 > dy)
                {
                    err += dy;
                    x0 += sx;
                } /* e_xy+e_x > 0 */

                if (e2 > dx)
                {
                    err += dx;
                    y0 += sy;
                } /* e_xy+e_y < 0 */

                if (e2 > dz)
                {
                    err += dz;
                    z0 += sz;
                }
            }
        }

        #region Converter

        private static OrientationFlags FindEdge(Vector2 point, OrientationFlags upper, OrientationFlags lower, OrientationFlags left, OrientationFlags right)
        {
            if (point.X > point.Y)
                return 1f - point.X > point.Y ? upper : right;

            return 1f - point.X > point.Y ? left : lower;
        }

        private static OrientationFlags FindCorner(Vector2 point, OrientationFlags upperLeftCorner, OrientationFlags upperRightCorner, OrientationFlags lowerLeftCorner, OrientationFlags lowerRightCorner)
        {
            if (point.X < 0.5f)
                return point.Y < 0.5f ? upperLeftCorner : lowerLeftCorner;

            return point.Y < 0.5f ? upperRightCorner : lowerRightCorner;
        }

        #endregion
    }
}