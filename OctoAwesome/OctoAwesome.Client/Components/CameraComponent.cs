using System;
using engenious;
using engenious.Helper;

namespace OctoAwesome.Client.Components
{
    internal sealed class CameraComponent : DrawableGameComponent
    {
        private readonly PlayerComponent player;


        public CameraComponent(OctoGame game)
            : base(game)
        {
            player = game.Player;
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

        public void RecreateProjection()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (player == null || player.CurrentEntity == null)
                return;

            var entity = player.CurrentEntity;
            var head = player.CurrentEntityHead;
            var position = player.Position;

            CameraChunk = position.Position.ChunkIndex;

            CameraPosition = position.Position.LocalPosition + head.Offset;
            CameraUpVector = new Vector3(0, 0, 1f);

            var height = (float) Math.Sin(head.Tilt);
            var distance = (float) Math.Cos(head.Tilt);

            var lookX = (float) Math.Cos(head.Angle) * distance;
            var lookY = -(float) Math.Sin(head.Angle) * distance;

            var strafeX = (float) Math.Cos(head.Angle + MathHelper.PiOver2);
            var strafeY = -(float) Math.Sin(head.Angle + MathHelper.PiOver2);

            CameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY), new Vector3(lookX, lookY, height));

            View = Matrix.CreateLookAt(
                CameraPosition,
                new Vector3(
                    CameraPosition.X + lookX,
                    CameraPosition.Y + lookY,
                    CameraPosition.Z + height),
                CameraUpVector);

            MinimapView = Matrix.CreateLookAt(
                new Vector3(CameraPosition.X, CameraPosition.Y, 100),
                new Vector3(
                    position.Position.LocalPosition.X,
                    position.Position.LocalPosition.Y),
                new Vector3(
                    (float) Math.Cos(head.Angle),
                    (float) Math.Sin(-head.Angle)));

            float centerX = GraphicsDevice.Viewport.Width / 2;
            float centerY = GraphicsDevice.Viewport.Height / 2;

            var nearPoint =
                GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY), Projection, View, Matrix.Identity);
            var farPoint =
                GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 1f), Projection, View, Matrix.Identity);
            var direction = farPoint - nearPoint;
            direction.Normalize();
            PickRay = new Ray(nearPoint, direction);
            Frustum = new BoundingFrustum(Projection * View);
        }
    }
}