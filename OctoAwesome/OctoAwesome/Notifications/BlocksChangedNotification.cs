using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Notifications
{
    public sealed class BlocksChangedNotification : SerializableNotification, IChunkNotification
    {
        public ICollection<BlockInfo> BlockInfos { get; set; }
        public Index3 ChunkPos { get; internal set; }
        public int Planet { get; internal set; }

        public override void Deserialize(BinaryReader reader)
        {
            if (reader.ReadByte() != (byte)BlockNotificationType.BlocksChanged) //Read type of the notification
                throw new InvalidCastException("this is the wrong type of notification");

            ChunkPos = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

            Planet = reader.ReadInt32();
            var count = reader.ReadInt32();
            var list = new List<BlockInfo>(count);

            for (var i = 0; i < count; i++)
                list.Add(new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadUInt16(), reader.ReadInt32()));
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)BlockNotificationType.BlocksChanged); //indicate that this is a multi Block Notification
            writer.Write(ChunkPos.X);
            writer.Write(ChunkPos.Y);
            writer.Write(ChunkPos.Z);
            writer.Write(Planet);

            writer.Write(BlockInfos.Count);
            foreach (var block in BlockInfos)
            {
                writer.Write(block.Position.X);
                writer.Write(block.Position.Y);
                writer.Write(block.Position.Z);
                writer.Write(block.Block);
                writer.Write(block.Meta);
            }
        }

        protected override void OnRelease()
        {
            BlockInfos = default;
            ChunkPos = default;
            Planet = default;

            base.OnRelease();
        }
    }
}