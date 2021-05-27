using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OctoAwesome.Notifications
{
    public class NotificationChannelCollection : IEnumerable<KeyValuePair<string, ObserverHashSet>>
    {
<<<<<<< HEAD
        private readonly Dictionary<string, ObserverHashSet> _internalDictionary;
        private readonly SemaphoreSlim _addSemaphore;
        
        public ObserverHashSet this[string channel] => _internalDictionary[channel];

        public ICollection<string> Channels => _internalDictionary.Keys;

        public int Count => _internalDictionary.Count;

        public Dictionary<string, ObserverHashSet>.ValueCollection Values => _internalDictionary.Values;

        public NotificationChannelCollection()
        {
            _internalDictionary = new Dictionary<string, ObserverHashSet>();
            _addSemaphore = new SemaphoreSlim(1,1);
=======
        public ObserverHashSet this[string channel] => internalDictionary[channel];

        public ICollection<string> Channels => internalDictionary.Keys;

        public int Count => internalDictionary.Count;

        public Dictionary<string, ObserverHashSet>.ValueCollection Values => internalDictionary.Values;

        private readonly Dictionary<string, ObserverHashSet> internalDictionary;
        private readonly SemaphoreSlim addSemaphore;

        public NotificationChannelCollection()
        {
            internalDictionary = new Dictionary<string, ObserverHashSet>();
            addSemaphore = new SemaphoreSlim(1,1);
>>>>>>> feature/performance
        }

        public void Add(string channel, INotificationObserver value)
        {
<<<<<<< HEAD
            _addSemaphore.Wait();
            
            if (_internalDictionary.TryGetValue(channel, out ObserverHashSet hashset))
=======
            addSemaphore.Wait();
            if (internalDictionary.TryGetValue(channel, out ObserverHashSet hashset))
>>>>>>> feature/performance
            {
                using (hashset.Wait())
                    hashset.Add(value);
            }
            else
            {
<<<<<<< HEAD
                _internalDictionary.Add(channel, new ObserverHashSet { value });
            }
            _addSemaphore.Release();
        }

        public void Clear() => _internalDictionary.Clear();

        public bool Contains(INotificationObserver item) => _internalDictionary.Values.Any(i => i == item);
       
        public bool Contains(string key) => _internalDictionary.ContainsKey(key);

        public Dictionary<string, ObserverHashSet>.Enumerator GetEnumerator() => _internalDictionary.GetEnumerator();

        public bool Remove(string key) => _internalDictionary.Remove(key);
        
=======
                internalDictionary.Add(channel, new ObserverHashSet { value });
            }
            addSemaphore.Release();
        }

        public void Clear()
            => internalDictionary.Clear();

        public bool Contains(INotificationObserver item)
            => internalDictionary.Values.Any(i => i == item);
        public bool Contains(string key)
            => internalDictionary.ContainsKey(key);

        public Dictionary<string, ObserverHashSet>.Enumerator GetEnumerator()
            => internalDictionary.GetEnumerator();

        public bool Remove(string key)
            => internalDictionary.Remove(key);
>>>>>>> feature/performance
        public bool Remove(INotificationObserver item)
        {
            var returnValue = false;

<<<<<<< HEAD
            foreach (var hashSet in _internalDictionary.Values)
=======
            foreach (ObserverHashSet hashSet in internalDictionary.Values)
>>>>>>> feature/performance
            {
                using (hashSet.Wait())
                    returnValue = returnValue ? returnValue : hashSet.Remove(item);
            }

            return returnValue;
        }
        public bool Remove(string key, INotificationObserver item)
        {
<<<<<<< HEAD
            var hashSet = _internalDictionary[key];
=======
            ObserverHashSet hashSet = internalDictionary[key];
>>>>>>> feature/performance
            bool returnValue;

            using (hashSet.Wait())
                returnValue = hashSet.Remove(item);

            return returnValue;
        }

<<<<<<< HEAD
        public bool TryGetValue(string key, out ObserverHashSet value) => _internalDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _internalDictionary.GetEnumerator();
        
        IEnumerator<KeyValuePair<string, ObserverHashSet>> IEnumerable<KeyValuePair<string, ObserverHashSet>>.GetEnumerator() => _internalDictionary.GetEnumerator();
=======
        public bool TryGetValue(string key, out ObserverHashSet value)
            => internalDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => internalDictionary.GetEnumerator();
        IEnumerator<KeyValuePair<string, ObserverHashSet>> IEnumerable<KeyValuePair<string, ObserverHashSet>>.GetEnumerator()
            => internalDictionary.GetEnumerator();
>>>>>>> feature/performance
    }
}
