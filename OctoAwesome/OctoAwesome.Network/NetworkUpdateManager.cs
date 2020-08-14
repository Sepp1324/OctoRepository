using OctoAwesome.Notifications;
using System;

namespace OctoAwesome.Network
{
    public class NetworkUpdateManager : IObserver<Package>
    {
        private readonly Client client;
        private readonly IUpdateHub updateHub;
        private readonly IDisposable subscription;

        public NetworkUpdateManager(Client client, IUpdateHub updateHub)
        {
            this.client = client;
            this.updateHub = updateHub;
            subscription = client.Subscribe(this);
        }

        public void OnNext(Package package)
        {
            switch (package.Command)
            {
                case (ushort)OfficialCommand.NewEntity:
                    updateHub.Push(new EntityNotification());
                    break;
                case (ushort)OfficialCommand.RemoveEntity:
                    break;
                default: 
                    break;

            }
        }

        public void OnError(Exception error) => throw error;

        public void OnCompleted() => subscription.Dispose();
    }
}