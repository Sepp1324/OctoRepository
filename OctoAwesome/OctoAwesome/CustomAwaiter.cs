using System.IO;
using System.Threading;

namespace OctoAwesome
{
    public class CustomAwaiter
    {
        private ISerializable _result;
        
        private ManualResetEventSlim _manualResetEventSlim;
        
        public CustomAwaiter()
        {
            _manualResetEventSlim = new ManualResetEventSlim(false);
        }

        public T WaitOn<T>() where T : ISerializable
        {
            _manualResetEventSlim.Wait();
            return (T) _result;
        }
        
        public void SetResult(ISerializable result)
        {
            _result = result;
            _manualResetEventSlim.Set();
        }
    }

    public class CustomAwaiter<T> : CustomAwaiter where T : ISerializable
    {
        public T Result => WaitOn<T>();
    }
}