using System.Collections.Generic;
using System.IO;
using System.Linq;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Client.Components
{
    internal sealed class EntityComponent : GameComponent
    {
        private readonly BasicEffect _effect;
        private readonly GraphicsDevice _graphicsDevice;


        private readonly Dictionary<string, ModelInfo> _models = new();

        public EntityComponent(OctoGame game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            _graphicsDevice = game.GraphicsDevice;

            _effect = new BasicEffect(_graphicsDevice);
        }

        private SimulationComponent Simulation { get; }

        private List<Entity> Entities { get; set; }

        public void Draw(Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
        {
            _effect.Projection = projection;
            _effect.View = view;
            _effect.TextureEnabled = true;
            _graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            using var writer = File.AppendText(Path.Combine(".", "render.log"));
            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var entity in Entities)
                {
                    if (!entity.Components.ContainsComponent<RenderComponent>()) continue;

                    var rendercomp = entity.Components.GetComponent<RenderComponent>();


                    if (!_models.TryGetValue(rendercomp.Name, out var modelinfo))
                        modelinfo = new ModelInfo
                        {
                            Render = true,
                            Model = Game.Content.Load<Model>(rendercomp.ModelName),
                            Texture = Game.Content.Load<Texture2D>(rendercomp.TextureName)
                        };

                    if (!modelinfo.Render)
                        continue;

                    var positioncomp = entity.Components.GetComponent<PositionComponent>();
                    var position = positioncomp.Position;
                    var body = entity.Components.GetComponent<BodyComponent>();

                    var head = new HeadComponent();
                    if (entity.Components.ContainsComponent<HeadComponent>())
                        head = entity.Components.GetComponent<HeadComponent>();

                    var shift = chunkOffset.ShortestDistanceXY(
                        position.ChunkIndex, planetSize);

                    var rotation = MathHelper.WrapAngle(positioncomp.Direction + MathHelper.ToRadians(rendercomp.BaseZRotation));

                    var world = Matrix.CreateTranslation(
                        shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                        shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                        shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z) * Matrix.CreateScaling(body.Radius * 2, body.Radius * 2, body.Height) * Matrix.CreateRotationZ(rotation);
                    _effect.World = world;
                    modelinfo.Model.Transform = world;

                    modelinfo.Model.Draw(_effect, modelinfo.Texture);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Simulation?.Simulation == null)
                return;

            var simulation = Simulation.Simulation;

            if (!(simulation.State == SimulationState.Running || simulation.State == SimulationState.Paused))
                return;

            Entities.Clear();
            foreach (var item in simulation.Entities.Where(item => item.Components.ContainsComponent<PositionComponent>()))
                Entities.Add(item);

            //base.Update(gameTime);
        }

        private struct ModelInfo
        {
            public bool Render;
            public Texture2D Texture;
            public Model Model;
        }
    }
}