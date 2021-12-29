using System;
using engenious;
using engenious.Helper;

namespace OctoAwesome.Client.Components
{
    internal sealed class CameraComponent : DrawableGameComponent
    {
        private readonly PlayerComponent _player;

        public CameraComponent(OctoGame game) : base(game)
        {
            _player = game.Player;
        }

        public Index3 CameraChunk { get; private set; }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraUpVector { get; private set; }

        public Matrix View { get; private set; }

        public Matrix MinimapView { get; private set; }

        public Matrix Projection { get; private set; }

        public Ray PickRay { get; private set; }

        public BoundingFrustum Frustum { get; private set; }

        public float NearPlaneDistance => 0.1f;

        public float FarPlaneDistance => 10000.0f;

        public override void Initialize()
        {
            base.Initialize();

            RecreateProjection();
        }

        public void RecreateProjection() => Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f); //TODO: 1000?

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (_player?.CurrentEntity == null)
                return;

            var entity = _player.CurrentEntity;
            var head = _player.CurrentEntityHead;
            var position = _player.Position;

            CameraChunk = position.Position.ChunkIndex;

            CameraPosition = position.Position.LocalPosition + head.Offset;
            CameraUpVector = new(0, 0, 1f);

            var height = (float)Math.Sin(head.Tilt);
            var distance = (float)Math.Cos(head.Tilt);

            var lookX = (float)Math.Cos(head.Angle) * distance;
            var lookY = -(float)Math.Sin(head.Angle) * distance;

            var strafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
            var strafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);

            CameraUpVector = Vector3.Cross(new(strafeX, strafeY), new(lookX, lookY, height));

            View = Matrix.CreateLookAt(
                CameraPosition,
                new(
                    CameraPosition.X + lookX,
                    CameraPosition.Y + lookY,
                    CameraPosition.Z + height),
                CameraUpVector);

            MinimapView = Matrix.CreateLookAt(
                new(CameraPosition.X, CameraPosition.Y, 100),
                new(
                    position.Position.LocalPosition.X,
                    position.Position.LocalPosition.Y),
                new(
                    (float)Math.Cos(head.Angle),
                    (float)Math.Sin(-head.Angle)));

            float centerX = GraphicsDevice.Viewport.Width / 2;
            float centerY = GraphicsDevice.Viewport.Height / 2;

            var nearPoint =
                GraphicsDevice.Viewport.Unproject(new(centerX, centerY), Projection, View, Matrix.Identity);
            var farPoint =
                GraphicsDevice.Viewport.Unproject(new(centerX, centerY, 1f), Projection, View, Matrix.Identity);
            var direction = farPoint - nearPoint;
            direction.Normalize();
            PickRay = new(nearPoint, direction);
            Frustum = new(Projection * View);
        }
    }
}