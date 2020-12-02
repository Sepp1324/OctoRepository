using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OctoAwesome.Threading;

namespace OctoAwesome.Client.Screens
{
    public sealed class QuoteProvider
    {
        private readonly FileInfo fileInfo;
        private readonly Random random;
        private bool loaded;
        private string[] quotes;

        private readonly LockedSemaphore _lockedSemaphore;

        public QuoteProvider(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            random = new Random();
            _lockedSemaphore = new LockedSemaphore(1, 1);
        }

        public string GetRandomQuote()
        {
            using (_lockedSemaphore.Wait())
            {
                Load();
                return quotes[random.Next(0, quotes.Length)];
            }
        }

        private void Load()
        {
            if (loaded)
                return;

            loaded = true;
            quotes = File.ReadAllLines(fileInfo.FullName);
        }
    }
}
