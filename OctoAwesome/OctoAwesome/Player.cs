﻿using System.IO;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;

namespace OctoAwesome
{
    /// <summary>
    /// Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    public sealed class Player : Entity
    {
        private readonly IPool<EntityNotification> _entityNotificationPool;
        
        /// <summary>
        /// Die Reichweite des Spielers, in der er mit Spielelementen wie <see cref="Block"/> und <see cref="Entity"/> interagieren kann
        /// </summary>
        public const int SelectionRange = 8;

        /// <summary>
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player() : base() => _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();

        protected override void OnInitialize(IResourceManager manager)
        {
            //Cache = new LocalChunkCache(manager.GlobalChunkCache, false, 2, 1);
        }

        /// <summary>
        /// Serialisiert den Player mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
<<<<<<< HEAD
        public override void Serialize(BinaryWriter writer) => base.Serialize(writer); // Entity
=======
        public override void Serialize(BinaryWriter writer)
            => base.Serialize(writer); // Entity
>>>>>>> feature/performance

        /// <summary>
        /// Deserialisiert den Player aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
<<<<<<< HEAD
        public override void Deserialize(BinaryReader reader) => base.Deserialize(reader); // Entity
=======
        public override void Deserialize(BinaryReader reader)
            => base.Deserialize(reader); // Entity
>>>>>>> feature/performance

        public override void OnUpdate(SerializableNotification notification)
        {
            base.OnUpdate(notification);

            var entityNotification = _entityNotificationPool.Get();
            entityNotification.Entity = this;
            entityNotification.Type = EntityNotification.ActionType.Update;
            entityNotification.Notification = notification as PropertyChangedNotification;

            Simulation?.OnUpdate(entityNotification);
            entityNotification.Release();
        }

    }
}
