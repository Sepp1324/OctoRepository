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
            if (reader.ReadByte() != (byte)BlockNotificationType.BlocksChanged)
                throw new InvalidCastException("Wrong type of Notification");

            ChunkPos = new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            Planet = reader.ReadInt32();

            var count = reader.ReadInt32();
            var list = new List<BlockInfo>(count);

            for (var i = 0; i < count; i++)
                list.Add(new BlockInfo(x: reader.ReadInt32(), y: reader.ReadInt32(), z: reader.ReadInt32(), block: reader.ReadUInt16(), meta: reader.ReadInt32()));
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)BlockNotificationType.BlocksChanged); //indicates that this is a multiple BlockNotification
            writer.Write(ChunkPos.X);
            writer.Write(ChunkPos.Y);
            writer.Write(ChunkPos.Z);
            writer.Write(Planet);
            writer.Write(BlockInfos.Count);

            foreach (var blockInfo in BlockInfos)
            {
                writer.Write(blockInfo.Position.X);
                writer.Write(blockInfo.Position.Y);
                writer.Write(blockInfo.Position.Z);
                writer.Write(blockInfo.Block);
                writer.Write(blockInfo.Meta);
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
