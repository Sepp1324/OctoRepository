using System;

namespace OctoAwesome.Database.Threading
{
    public readonly struct DatabaseLock : IDisposable
    {
        public void Dispose() => throw new NotImplementedException();
    }
}
