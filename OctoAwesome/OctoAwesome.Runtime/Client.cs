﻿using System;
using System.ServiceModel;

namespace OctoAwesome.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Client : IClient
    {
        public IClientCallback Callback { get; private set; }

        public Guid ConnectionId { get; private set; }

        public string Playername { get; private set; }

        public Client()
        {
            Callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            ConnectionId = Guid.NewGuid();
        }

        [OperationBehavior]
        public Guid Connect(string playername)
        {
            Playername = playername;
            Server.Instance.Register(this);
            return ConnectionId;
        }

        [OperationBehavior]
        public void Disconnect()
        {
            Server.Instance.Deregister(this);
        }

        [OperationBehavior]
        public void Jump()
        {
            throw new NotImplementedException();
        }
    }
}
