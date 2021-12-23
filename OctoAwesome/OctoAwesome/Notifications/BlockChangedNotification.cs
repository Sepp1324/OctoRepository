using System;
using System.IO;

namespace OctoAwesome.Notifications
{
    /// <summary>
    ///     Notification for Block CHanges
    /// </summary>
    public sealed class BlockChangedNotification : SerializableNotification, IChunkNotification
    {
        /// <summary>
        ///     Information of Block
        /// </summary>
        public BlockInfo BlockInfo { get; set; }

        /// <summary>
        ///     ChunkPosition of Block
        /// </summary>
        public Index3 ChunkPos { get; internal set; }

        /// <summary>
        ///     Current Planet of Block
        /// </summary>
        public int Planet { get; internal set; }

        /// <summary>
        ///     Deserialize Block with given <see cref="BinaryReader" />
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="InvalidCastException"></exception>
        public override void Deserialize(BinaryReader reader)
        {
            if (reader.ReadByte() != (byte)BlockNotificationType.BlockChanged) //Read type of the notification
                throw new InvalidCastException("this is the wrong type of notification");

            BlockInfo = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadUInt16(),
                reader.ReadInt32());
            ChunkPos = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            Planet = reader.ReadInt32();
        }

        /// <summary>
        ///     Serialize Block with given <see cref="BinaryWriter" />
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)BlockNotificationType.BlockChanged); //indicate that this is a single Block Notification

            writer.Write(BlockInfo.Position.X);
            writer.Write(BlockInfo.Position.Y);
            writer.Write(BlockInfo.Position.Z);
            writer.Write(BlockInfo.Block);
            writer.Write(BlockInfo.Meta);

            writer.Write(ChunkPos.X);
            writer.Write(ChunkPos.Y);
            writer.Write(ChunkPos.Z);
            writer.Write(Planet);
        }

        /// <summary>
        ///     Event for Block-Release
        /// </summary>
        protected override void OnRelease()
        {
            BlockInfo = default;
            ChunkPos = default;
            Planet = default;

            base.OnRelease();
        }
    }
}