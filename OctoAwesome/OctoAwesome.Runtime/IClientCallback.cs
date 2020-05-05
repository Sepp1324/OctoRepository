using System.ServiceModel;

namespace OctoAwesome.Runtime
{
    internal interface IClientCallback
    {
        [OperationContract(IsInitiating = false, IsTerminating = false, IsOneWay = true)]
        void Relocation(int x, int y, int z);
    }
}
