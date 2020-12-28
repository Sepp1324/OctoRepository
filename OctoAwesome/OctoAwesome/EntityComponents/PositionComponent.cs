using engenious;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    public sealed class PositionComponent : EntityComponent
    {
        private readonly IPool<PropertyChangedNotification> _propertyChangedNotificationPool;
        private readonly IResourceManager _resourceManager;

        private Coordinate _position;
        private bool _posUpdate;

        public PositionComponent()
        {
            Sendable = true;
            _resourceManager = TypeContainer.Get<IResourceManager>();
            _propertyChangedNotificationPool = TypeContainer.Get<IPool<PropertyChangedNotification>>();
        }

        public Coordinate Position
        {
            get => _position;
            set
            {
                var valueBlockX = ((int) (value.BlockPosition.X * 100)) / 100f;
                var valueBlockY = ((int) (value.BlockPosition.Y * 100)) / 100f;
                var positionBlockX = ((int) (_position.BlockPosition.X * 100)) / 100f;
                var positionBlockY = ((int) (_position.BlockPosition.Y * 100)) / 100f;

                _posUpdate = valueBlockX != positionBlockX || valueBlockY != positionBlockY || _position.BlockPosition.Z != value.BlockPosition.Z;

                SetValue(ref _position, value);
                TryUpdatePlanet(value.Planet);
            }
        }

        public float Direction { get; set; }
        public IPlanet Planet { get; private set; }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            // Position
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            // Position
            var planet = reader.ReadInt32();
            var blockX = reader.ReadInt32();
            var blockY = reader.ReadInt32();
            var blockZ = reader.ReadInt32();
            var posX = reader.ReadSingle();
            var posY = reader.ReadSingle();
            var posZ = reader.ReadSingle();

            _position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));
            TryUpdatePlanet(planet);
        }

        private bool TryUpdatePlanet(int planetId)
        {
            if (Planet != null && Planet.Id == planetId)
                return false;

            Planet = _resourceManager.GetPlanet(planetId);
            return true;
        }

        protected override void OnPropertyChanged<T>(T value, string callerName)
        {
            base.OnPropertyChanged(value, callerName);

            if (callerName == nameof(Position) && _posUpdate)
            {
                var updateNotification = _propertyChangedNotificationPool.Get();

                updateNotification.Issuer = nameof(PositionComponent);
                updateNotification.Property = callerName;

                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    Serialize(writer);
                    updateNotification.Value = stream.ToArray();
                }

                Update(updateNotification);
            }
        }

        public override void OnUpdate(SerializableNotification notification)
        {
            base.OnUpdate(notification);

            if (notification is PropertyChangedNotification changedNotification)
            {
                if (changedNotification.Issuer == nameof(PositionComponent))
                {
                    if (changedNotification.Property == nameof(Position))
                    {
                        using (var stream = new MemoryStream(changedNotification.Value))
                        using (var reader = new BinaryReader(stream))
                        {
                            Deserialize(reader);
                        }
                    }
                }
            }
        }
    }
}