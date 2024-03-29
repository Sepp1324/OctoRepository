﻿using System.IO;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;

namespace OctoAwesome
{
    /// <summary>
    ///     Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    public sealed class Player : Entity
    {
        /// <summary>
        ///     Die Reichweite des Spielers, in der er mit Spielelementen wie <see cref="Block" /> und <see cref="Entity" />
        ///     interagieren kann
        /// </summary>
        public const int SELECTION_RANGE = 8;

        private readonly IPool<EntityNotification> _entityNotificationPool;

        /// <summary>
        ///     Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player() => _entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();

        /// <summary>
        ///     Serialisiert den Player mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            // Entity
        }

        /// <summary>
        ///     Deserialisiert den Player aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            // Entity
        }

        public override void OnNotification(SerializableNotification notification)
        {
            base.OnNotification(notification);

            var entityNotification = _entityNotificationPool.Get();
            entityNotification.Entity = this;
            entityNotification.Type = EntityNotification.ActionType.Update;
            entityNotification.Notification = notification as PropertyChangedNotification;

            Simulation?.OnUpdate(entityNotification);
            entityNotification.Release();
        }
    }
}