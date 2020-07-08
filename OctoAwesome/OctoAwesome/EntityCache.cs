using System.Collections.Generic;

namespace OctoAwesome
{
    public sealed class EntityCache
    {
        private Dictionary<Index2, int> referenceCounter = new Dictionary<Index2, int>();

        private List<X> Entries { get; set; }

        public EntityCache()
        {
            Entries = new List<X>();
        }

        public void Update()
        {
            foreach (var Entity in Entries)
            {
                Entity.Update();
            }
        }

        public void Subscribe(Index2 index)
        {
            if(!referenceCounter.ContainsKey(index))
            {
                referenceCounter.Add(index, 0);
                //TODO: Entities aus diesem Chunk laden
            }
            referenceCounter[index]++;
        }

        public void Unsubscribe(Index2 index)
        {
            referenceCounter[index]--;

            if(referenceCounter[index] <= 0)
            {
                //TODO: Entities ausladen
                referenceCounter.Remove(index);
            }
        }

        private class X
        {
            public Index2 Position { get; set; }

            public Entity Entity { get; set; }

            public LocalChunkCache Cache { get; set; }

            public void Update()
            {
                //TODO: Entitymovement
                //TODO: Chunk Move
            }
        }
    }
}
