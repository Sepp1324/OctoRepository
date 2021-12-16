using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OctoAwesome.Notifications
{
    public class NotificationChannelCollection : IEnumerable<KeyValuePair<string, ObserverHashSet>>
    {
        private readonly SemaphoreSlim addSemaphore;

        private readonly Dictionary<string, ObserverHashSet> internalDictionary;

        public NotificationChannelCollection()
        {
            internalDictionary = new Dictionary<string, ObserverHashSet>();
            addSemaphore = new SemaphoreSlim(1, 1);
        }

        public ObserverHashSet this[string channel] => internalDictionary[channel];

        public ICollection<string> Channels => internalDictionary.Keys;

        public int Count => internalDictionary.Count;

        public Dictionary<string, ObserverHashSet>.ValueCollection Values => internalDictionary.Values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, ObserverHashSet>> IEnumerable<KeyValuePair<string, ObserverHashSet>>.
            GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        public void Add(string channel, INotificationObserver value)
        {
            addSemaphore.Wait();
            if (internalDictionary.TryGetValue(channel, out var hashset))
                using (hashset.Wait())
                {
                    hashset.Add(value);
                }
            else
                internalDictionary.Add(channel, new ObserverHashSet { value });

            addSemaphore.Release();
        }

        public void Clear()
        {
            internalDictionary.Clear();
        }

        public bool Contains(INotificationObserver item)
        {
            return internalDictionary.Values.Any(i => i == item);
        }

        public bool Contains(string key)
        {
            return internalDictionary.ContainsKey(key);
        }

        public Dictionary<string, ObserverHashSet>.Enumerator GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return internalDictionary.Remove(key);
        }

        public bool Remove(INotificationObserver item)
        {
            var returnValue = false;

            foreach (var hashSet in internalDictionary.Values)
                using (hashSet.Wait())
                {
                    returnValue = returnValue ? returnValue : hashSet.Remove(item);
                }

            return returnValue;
        }

        public bool Remove(string key, INotificationObserver item)
        {
            var hashSet = internalDictionary[key];
            bool returnValue;

            using (hashSet.Wait())
            {
                returnValue = hashSet.Remove(item);
            }

            return returnValue;
        }

        public bool TryGetValue(string key, out ObserverHashSet value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }
    }
}