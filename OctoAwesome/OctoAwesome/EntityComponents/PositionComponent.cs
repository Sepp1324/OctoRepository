using System.IO;
using OctoAwesome.Components;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// PositionComponent
    /// </summary>
    public sealed class PositionComponent : InstanceComponent<INotificationSubject<SerializableNotification>>, IEntityComponent, IFunctionalBlockComponent
    {
        private readonly IPool<PropertyChangedNotification> _propertyChangedNotificationPool;
        private readonly IResourceManager _resourceManager;

        private IPlanet _planet;
        private Coordinate _position;
        private bool _posUpdate;

        /// <summary>
        /// Direction
        /// </summary>
        public float Direction { get; set; }

        /// <summary>
        /// Current Planet
        /// </summary>
        public IPlanet Planet { get => _planet ??= TryGetPlanet(_position.Planet); private set => _planet = value; }

        /// <summary>
        /// PositionComponent
        /// </summary>
        public PositionComponent()
        {
            Sendable = true;
            _resourceManager = TypeContainer.Get<IResourceManager>();
            _propertyChangedNotificationPool = TypeContainer.Get<IPool<PropertyChangedNotification>>();
        }

        /// <summary>
        /// Position
        /// </summary>
        public Coordinate Position
        {
            get => _position;
            set
            {
                var valueBlockX = (int)(value.BlockPosition.X * 100) / 100f;
                var valueBlockY = (int)(value.BlockPosition.Y * 100) / 100f;
                var positionBlockX = (int)(_position.BlockPosition.X * 100) / 100f;
                var positionBlockY = (int)(_position.BlockPosition.Y * 100) / 100f;

                _posUpdate = valueBlockX != positionBlockX || valueBlockY != positionBlockY || _position.BlockPosition.Z != value.BlockPosition.Z;

                SetValue(ref _position, value);
                _planet = TryGetPlanet(value.Planet);
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
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

            _position = new(planet, new(blockX, blockY, blockZ), new(posX, posY, posZ));
        }

        private IPlanet TryGetPlanet(int planetId)
        {
            if (_planet != null && _planet.Id == planetId)
                return _planet;

            return _resourceManager.GetPlanet(planetId);
        }

        protected override void OnPropertyChanged<T>(T value, string callerName)
        {
            base.OnPropertyChanged(value, callerName);

            if (callerName != nameof(Position) || !_posUpdate) 
                return;

            var updateNotification = _propertyChangedNotificationPool.Get();

            updateNotification.Issuer = nameof(PositionComponent);
            updateNotification.Property = callerName;

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                Serialize(writer);
                updateNotification.Value = stream.ToArray();
            }

            Push(updateNotification);
        }

        public override void OnNotification(SerializableNotification notification)
        {
            base.OnNotification(notification);

            if (notification is PropertyChangedNotification changedNotification)
                if (changedNotification.Issuer == nameof(PositionComponent))
                    if (changedNotification.Property == nameof(Position))
                        using (var stream = new MemoryStream(changedNotification.Value))
                        using (var reader = new BinaryReader(stream))
                        {
                            Deserialize(reader);
                        }
        }
    }
}