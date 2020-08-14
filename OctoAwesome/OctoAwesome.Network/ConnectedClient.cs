﻿using OctoAwesome.Network.ServerNotifications;
using OctoAwesome.Notifications;
using System;
using System.Net.Sockets;

namespace OctoAwesome.Network
{
    public class ConnectedClient : BaseClient, IUpdateSubscriber
    {
        public IDisposable ProviderSubscription { get; set; }

        public IDisposable ServerSubscription { get; set; }

        public ConnectedClient(Socket socket) : base(socket)
        {

        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            Socket.Close();
            throw error;
        }

        public void OnNext(Notification value)
        {
            switch(value)
            {
                case ServerDataNotification serverDataNotification:
                    break;
                default:
                    break;
            }
        }
    }
}