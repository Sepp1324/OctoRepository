using engenious;
using engenious.Graphics;
using engenious.Helper;
using OctoAwesome.EntityComponents;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Client.Components
{
    internal sealed class EntityComponent : GameComponent
    {
        private struct ModelInfo
        {
            public bool render;
            public Texture2D texture;
            public Model model;
        }

        private readonly GraphicsDevice _graphicsDevice;
        private readonly BasicEffect _effect;

        public SimulationComponent Simulation { get; private set; }


        private Dictionary<string, ModelInfo> models = new Dictionary<string, ModelInfo>();

        public List<Entity> Entities { get; set; }

        public EntityComponent(OctoGame game, SimulationComponent simulation) : base(game)
        {
            Simulation = simulation;

            Entities = new List<Entity>();
            _graphicsDevice = game.GraphicsDevice;

            _effect = new BasicEffect(_graphicsDevice);
        }

        private int i = 0;
        public void Draw(Matrix view, Matrix projection, Index3 chunkOffset, Index2 planetSize)
        {
            _effect.Projection = projection;
            _effect.View = view;
            _effect.TextureEnabled = true;
            _graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            using (var writer = File.AppendText(Path.Combine(".", "render.log")))
                foreach (var pass in _effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    i++;

                    foreach (var entity in Entities)
                    {
                        if (!entity.Components.ContainsComponent<RenderComponent>())
                            continue;

                        var renderComp = entity.Components.GetComponent<RenderComponent>();


                        if (!models.TryGetValue(renderComp.Name, out var modelInfo))
                        {
                            modelInfo = new ModelInfo()
                            {
                                render = true,
                                model = Game.Content.Load<Model>(renderComp.ModelName),
                                texture = Game.Content.Load<Texture2D>(renderComp.TextureName),
                            };
                        }

                        if (!modelInfo.render)
                            continue;

                        var positionComp = entity.Components.GetComponent<PositionComponent>();
                        var position = positionComp.Position;
                        var body = entity.Components.GetComponent<BodyComponent>();

                        var head = new HeadComponent();

                        if (entity.Components.ContainsComponent<HeadComponent>())
                            head = entity.Components.GetComponent<HeadComponent>();

                        var shift = chunkOffset.ShortestDistanceXY(position.ChunkIndex, planetSize);

                        var rotation = MathHelper.WrapAngle(positionComp.Direction + MathHelper.ToRadians(renderComp.BaseZRotation));

                        var world = Matrix.CreateTranslation(
                            shift.X * Chunk.CHUNKSIZE_X + position.LocalPosition.X,
                            shift.Y * Chunk.CHUNKSIZE_Y + position.LocalPosition.Y,
                            shift.Z * Chunk.CHUNKSIZE_Z + position.LocalPosition.Z) * Matrix.CreateScaling(body.Radius * 2, body.Radius * 2, body.Height) * Matrix.CreateRotationZ(rotation);
                        _effect.World = world;
                        modelInfo.model.Transform = world;

                        modelInfo.model.Draw(_effect, modelInfo.texture);
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

            Entities = simulation.Entities.Where(i => i.Components.ContainsComponent<PositionComponent>()).ToList();
        }
    }
}
