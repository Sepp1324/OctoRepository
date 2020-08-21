using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;
using System.IO;

namespace OctoAwesome.Network
{
    public class NetworkUpdateManager : IObserver<Package>, INotificationObserver
    {
        private readonly Client client;
        private readonly IUpdateHub updateHub;
        private readonly IDisposable hubSubscription;
        private readonly IDisposable clientSubscription;
        private readonly IDefinitionManager definitionManager;

        public NetworkUpdateManager(Client client, IUpdateHub updateHub, IDefinitionManager manager)
        {
            this.client = client;
            this.updateHub = updateHub;

            hubSubscription = updateHub.Subscribe(this, DefaultChannels.NETWORK);
            clientSubscription = client.Subscribe(this);
            definitionManager = manager;
        }

        public void OnNext(Package package)
        {
            switch (package.Command)
            {
                case (ushort)OfficialCommand.EntityNotification:
                    var entityNotification = new EntityNotification();

                    using (var stream = new MemoryStream(package.Payload))
                    using (var reader = new BinaryReader(stream))
                    {
                        entityNotification.Deserialize(reader);
                    }
                    updateHub.Push(entityNotification, DefaultChannels.SIMULATION);
                    break;
                default:
                    break;
            }
        }

        public void OnError(Exception error) => throw error;

        public void OnCompleted() => clientSubscription.Dispose();

        public void OnNext(Notification value)
        {
            ushort command;
            switch (value)
            {
                case EntityNotification _:
                    command = (ushort)OfficialCommand.EntityNotification;
                    break;
                default:
                    return;
            }

            if (value is ISerializable notification)
            {
                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    notification.Serialize(writer, definitionManager);
                    client.SendPackage(new Package(command, (int)stream.Length)
                    {
                        Payload = stream.ToArray()
                    });
                }
            }
        }
    }
}