using System;
using System.IO;
using OctoAwesome.Threading;

namespace OctoAwesome.Client.Screens
{
    public sealed class QuoteProvider
    {
        private readonly FileInfo _fileInfo;
        private readonly Random _random;
        private bool _loaded;
        private string[] _quotes;

        private readonly LockedSemaphore _lockedSemaphore;

        public QuoteProvider(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            _random = new Random();
            _lockedSemaphore = new LockedSemaphore(1, 1);
        }

        public string GetRandomQuote()
        {
            using (_lockedSemaphore.Wait())
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
