using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Database
{
    public sealed class IdManager
    {
        private readonly Queue<int> _freeIds;
        private readonly HashSet<int> _reservedIds;
        private int _nextId;

        public IdManager() : this(Array.Empty<int>())
        {
        }
<<<<<<< HEAD
        
=======
>>>>>>> feature/performance
        public IdManager(IEnumerable<int> alreadyUsedIds)
        {
            if (alreadyUsedIds == null)
                alreadyUsedIds = Array.Empty<int>();

            _freeIds = new Queue<int>();
            _reservedIds = new HashSet<int>();

            var ids = alreadyUsedIds.Distinct().OrderBy(i => i).ToArray();
            if (ids.Length <= 0)
            {
                _nextId = 0;
                return;
            }
<<<<<<< HEAD
            _nextId = ids.Max();

            var ids2 = new List<int>(_nextId);
=======
            nextId = ids.Max();

            var ids2 = new List<int>(nextId);
>>>>>>> feature/performance
            ids2.AddRange(ids);

            for (var i = 0; i < _nextId; i++)
            {
                if (i >= ids2.Count || ids2[i] == i)
                    continue;

                ids2.Insert(i, i);
                _freeIds.Enqueue(i);
            }
        }

        public int GetId()
        {
            int id;

            do
            {
                id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _nextId++;
            } while (_reservedIds.Contains(id));

            return id;
        }

        public void ReleaseId(int id)
        {
            _freeIds.Enqueue(id);
            _reservedIds.Remove(id);
        }

<<<<<<< HEAD
        public void ReserveId(int id) => _reservedIds.Add(id);
=======
        public void ReserveId(int id) 
            => reservedIds.Add(id);
>>>>>>> feature/performance
    }
}
