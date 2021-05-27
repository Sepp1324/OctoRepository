using OctoAwesome.Threading;
using System;
using System.IO;

namespace OctoAwesome.Client.Screens
{
    public sealed class QuoteProvider
    {
<<<<<<< HEAD
        private readonly FileInfo _fileInfo;
        private readonly Random _random;
        private bool _loaded;
        private string[] _quotes;

        private readonly LockSemaphore semaphoreExtended;
=======
        private readonly FileInfo fileInfo;
        private readonly Random random;
        private bool loaded;
        private string[] quotes;
>>>>>>> feature/performance

        private readonly LockSemaphore semaphoreExtended;

        public QuoteProvider(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            _random = new Random();
            semaphoreExtended = new LockSemaphore(1, 1);
        }

        public string GetRandomQuote()
        {
            using (semaphoreExtended.Wait())
            {
                Load();
                return _quotes[_random.Next(0, _quotes.Length)];
            }
        }

        private void Load()
        {
            if (_loaded)
                return;

            _loaded = true;
            _quotes = File.ReadAllLines(_fileInfo.FullName);
        }
    }
}
