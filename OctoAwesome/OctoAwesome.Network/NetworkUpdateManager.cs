﻿using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;

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
                case (ushort)OfficialCommand.NewEntity:
                    var remoteEntity = Serializer.Deserialize<RemoteEntity>(package.Payload, definitionManager);
                    updateHub.Push(new EntityNotification()
                    {
                        Entity = remoteEntity,
                        Type = EntityNotification.ActionType.Add
                    }, DefaultChannels.SIMULATION);
                    break;
                case (ushort)OfficialCommand.RemoveEntity:
                    break;
                default:
                    break;
            }
        }

        public void OnError(Exception error) => throw error;

        public void OnCompleted() => clientSubscription.Dispose();

        public void OnNext(Notification value) => client.SendPackage(new Package());
    }
}