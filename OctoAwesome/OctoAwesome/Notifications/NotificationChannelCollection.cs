using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Notifications
{
    public class NotificationChannelCollection : IEnumerable
    {
        private readonly Dictionary<string, HashSet<INotificationObserver>> internalDictionary;

        public NotificationChannelCollection() => internalDictionary = new Dictionary<string, HashSet<INotificationObserver>>();

        public HashSet<INotificationObserver> this[string channel] => internalDictionary[channel];

        public ICollection<string> Channels => internalDictionary.Keys;

        public int Count => internalDictionary.Count;

        public void Add(string channel, INotificationObserver value)
        {
            if (internalDictionary.TryGetValue(channel, out var hashSet))
                hashSet.Add(value);
            else
                internalDictionary.Add(channel, new HashSet<INotificationObserver> { value });
        }

        public void Clear() => internalDictionary.Clear();

        public bool Contains(INotificationObserver item) => internalDictionary.Values.Any(i => i == item);

        public bool Contains(string key) => internalDictionary.ContainsKey(key);

        public IEnumerator GetEnumerator() => internalDictionary.GetEnumerator();

        public bool Remove(string key) => internalDictionary.Remove(key);

        public bool Remove(INotificationObserver item)
        {
            var retVal = false;

            foreach (var hashSet in internalDictionary.Values)
                retVal = retVal ? retVal : hashSet.Remove(item);
            return retVal;
        }

        public bool Remove(string key, INotificationObserver item) => internalDictionary[key].Remove(item);

        public bool TryGetValue(string key, out HashSet<INotificationObserver> value) => internalDictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
