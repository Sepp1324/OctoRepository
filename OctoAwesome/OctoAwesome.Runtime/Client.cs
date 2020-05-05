using System;
using System.ServiceModel;

namespace OctoAwesome.Runtime
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    internal class Client : IClient
    {
        [OperationBehavior]
        public void Connect(string playername)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public void Jump()
        {
            throw new NotImplementedException();
        }
    }
}
