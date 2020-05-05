﻿using System;
using System.ServiceModel;

namespace OctoAwesome.Runtime
{
    [ServiceContract(CallbackContract = typeof(IClientCallback), SessionMode = SessionMode.Required)]
    public interface IClient
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        Guid Connect(string playername);

        [OperationContract(IsInitiating = false, IsTerminating = true, IsOneWay = true)]
        void Disconnect();

        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Jump();
    }
}
