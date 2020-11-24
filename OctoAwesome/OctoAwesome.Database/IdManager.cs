using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Database
{
    /// <summary>
    /// IDManager für Entities
    /// </summary>
    public sealed class IdManager
    {
        private readonly Queue<int> _freeIds;
        private readonly HashSet<int> _reservedIds;
        private int _nextId;

        /// <summary>
        /// Konstruktor für IDManager
        /// </summary>
        public IdManager() : this(Array.Empty<int>())
        {
        }

        /// <summary>
        /// Konstruktor für IDManager
        /// </summary>
        /// <param name="alreadyUsedIds"></param>
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

        /// <summary>
        /// Liefert eine ID zurück
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            int id;

            do
            {
                id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _nextId++;
            } while (_reservedIds.Contains(id));
            return id;
        }

        /// <summary>
        /// Gibt eine ID wieder frei
        /// </summary>
        /// <param name="id"></param>
        public void ReleaseId(int id)
        {
            _freeIds.Enqueue(id);
            _reservedIds.Remove(id);
        }

        /// <summary>
        /// Reserviert eine ID
        /// </summary>
        /// <param name="id"></param>
        public void ReserveId(int id) => _reservedIds.Add(id);
    }
}
