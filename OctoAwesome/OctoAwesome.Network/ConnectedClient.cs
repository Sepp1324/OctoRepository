using System;
using System.Net.Sockets;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

namespace OctoAwesome.Network
{
    public sealed class ConnectedClient : BaseClient, INotificationObserver
    {
        private readonly PackagePool _packagePool;

        public ConnectedClient(Socket socket) : base(socket) => _packagePool = TypeContainer.Get<PackagePool>();

        public IDisposable NetworkChannelSubscription { get; set; }

        public IDisposable ServerSubscription { get; set; }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            Socket.Close();
            throw error;
        }

        public void OnNext(Notification value)
        {
            if (value.SenderId == Id)
                return;

            OfficialCommand command;
            byte[] payload;
            switch (value)
            {
                case EntityNotification entityNotification:
                    command = OfficialCommand.EntityNotification;
                    payload = Serializer.Serialize(entityNotification);
                    break;

                case BlocksChangedNotification _:
                case BlockChangedNotification _:
                    command = OfficialCommand.ChunkNotification;
                    payload = Serializer.Serialize(value as SerializableNotification);
                    break;
                default:
                    return;
            }

            BuildAndSendPackage(payload, command);
        }

        private void BuildAndSendPackage(byte[] data, OfficialCommand officialCommand)
        {
            var package = _packagePool.Get();
            package.Payload = data;
            package.Command = (ushort)officialCommand;
            SendPackageAndRelease(package);
        }
    }
}