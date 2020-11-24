using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Database
{
    public sealed class IdManager
    {
        private readonly Queue<int> _freeIds;
        private int _nextId;

        public IdManager() : this(Array.Empty<int>())
        {
        }

        public IdManager(IEnumerable<int> alreadyUsedIds)
        {
            if (alreadyUsedIds == null)
                alreadyUsedIds = Array.Empty<int>();
            
            _freeIds = new Queue<int>();

            var ids = alreadyUsedIds.OrderBy(i => i).ToArray();

            if (ids.Length <= 0)
            {
                _nextId = 0;
                return;
            }

            _nextId = ids.Max();

            var ids2 = new List<int>(_nextId);
            ids2.AddRange(ids);

            for (int i = 0; i < _nextId; i++)
            {
                if (i >= ids2.Count && ids2[i] == i)
                    continue;

                ids2.Insert(i, i);
                _freeIds.Enqueue(i);
            }
        }

        public int GetId()
        {
            if (_freeIds.Count > 0)
                return _freeIds.Dequeue();

            return _nextId++;
        }

        public void ReleaseId(int id) => _freeIds.Enqueue(id);
    }
}
