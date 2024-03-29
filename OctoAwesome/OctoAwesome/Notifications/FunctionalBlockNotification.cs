﻿using System;
using System.IO;
using OctoAwesome.Serialization;

namespace OctoAwesome.Notifications
{
    public sealed class FunctionalBlockNotification : SerializableNotification
    {
        public enum ActionType
        {
            None,
            Add,
            Remove,
            Update,
            Request
        }

        private FunctionalBlock _block;

        public FunctionalBlockNotification()
        {
        }

        public FunctionalBlockNotification(Guid id) : this() => BlockId = id;

        public ActionType Type { get; set; }

        public Guid BlockId { get; set; }

        public FunctionalBlock Block
        {
            get => _block;
            set
            {
                _block = value;
                BlockId = value?.Id ?? default;
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            Type = (ActionType)reader.ReadInt32();


            if (Type == ActionType.Add)
            {
            }
            //Block = Serializer.Deserialize()
            else
            {
                BlockId = new(reader.ReadBytes(16));
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((int)Type);

            if (Type == ActionType.Add)
            {
                var bytes = Serializer.Serialize(Block);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
            else
            {
                writer.Write(BlockId.ToByteArray());
            }
        }

        protected override void OnRelease()
        {
            Type = default;
            Block = default;

            base.OnRelease();
        }
    }
}