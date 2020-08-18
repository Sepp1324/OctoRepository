using System.Collections;
using System.Collections.Generic;

namespace OctoAwesome.Notifications
{
    public class NotificationChannelCollection : IEnumerable
    {
        private Dictionary<string, HashSet<INotificationObserver>> internalDictionary;

        public NotificationChannelCollection()
        {
            internalDictionary = new Dictionary<string, HashSet<INotificationObserver>>();
        }

        public INotificationObserver this[string key] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public ICollection<string> Keys => throw new System.NotImplementedException();

        public ICollection<INotificationObserver> Values => throw new System.NotImplementedException();

        public int Count => throw new System.NotImplementedException();

        public bool IsReadOnly => throw new System.NotImplementedException();

        public void Add(string key, INotificationObserver value)
        {
            throw new System.NotImplementedException();
        }

        public void Add(KeyValuePair<string, INotificationObserver> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, INotificationObserver> item)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, INotificationObserver>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, INotificationObserver>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, INotificationObserver> item)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out INotificationObserver value)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
