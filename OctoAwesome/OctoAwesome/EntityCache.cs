using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OctoAwesome
{
    public sealed class EntityCache
    {
        private object lockObject = new object();

        private Dictionary<PlanetIndex2, SubscriptionInfo> references = new Dictionary<PlanetIndex2, SubscriptionInfo>();

        private List<Entity> entities = new List<Entity>();

        private IResourceManager resourceManager;

        private List<X> Entries { get; set; }

        private Thread loaderThread;

        public EntityCache(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;

            Entries = new List<X>();

            loaderThread = new Thread(LoaderLoop);
            loaderThread.IsBackground = true;
            loaderThread.Priority = ThreadPriority.Lowest;
            loaderThread.Start();
        }

        public void Update()
        {
            foreach (var Entity in Entries)
            {
                Entity.Update();
            }
        }

        public void Subscribe(IPlanet planet, Index2 index, int activationRange)
        {
            int softBorder = 1;

            lock (lockObject)
            {

                for (int x = -activationRange - softBorder; x <= activationRange + softBorder; x++)
                {
                    for (int y = -activationRange - softBorder; y <= activationRange + softBorder; y++)
                    {
                        Index2 pos = new Index2(index.X + x, index.Y + y);
                        pos.NormalizeXY(planet.Size);

                        PlanetIndex2 i = new PlanetIndex2(planet.Id, index);

                        //Soft Reference
                        SubscriptionInfo reference;

                        if(!references.TryGetValue(i, out reference))
                        {
                            reference = new SubscriptionInfo()
                            {
                                LoaderState = LoaderState.ToLoad,
                                Position = i
                            };
                            references.Add(i, reference); 
                        }
                        else
                        {
                            switch (reference.LoaderState)
                            {
                                case LoaderState.ToUnload: reference.LoaderState = LoaderState.Ready; break;
                                case LoaderState.Unloading: reference.LoaderState = LoaderState.CancleUnload; break;
                            }
                        }

                        //Hard Reference
                        if (x >= -activationRange || x <= activationRange || y >= -activationRange || y <= activationRange)
                        {
                            reference.HardReference++;
                        }
                        reference.SoftReference++;
                    }
                }
            }
        }

        public void Unsubscribe(IPlanet planet, Index2 index, int activationRange)
        {
            int softBorder = 1;

            lock (lockObject)
            {
                for (int x = -activationRange - softBorder; x <= activationRange + softBorder; x++)
                {
                    for (int y = -activationRange - softBorder; y <= activationRange + softBorder; y++)
                    {
                        Index2 pos = new Index2(index.X + x, index.Y + y);
                        pos.NormalizeXY(planet.Size);

                        PlanetIndex2 i = new PlanetIndex2(planet.Id, index);

                        SubscriptionInfo reference = references[i];

                        //Hard Reference
                        if (x >= -activationRange || x <= activationRange || y >= -activationRange || y <= activationRange)
                        {
                            reference.HardReference--;
                        }
                        reference.SoftReference--;

                        //Soft Reference

                        if (reference.SoftReference <= 0)
                        {
                            //Entities ausladen
                            switch (reference.LoaderState)
                            {
                                case LoaderState.Ready: reference.LoaderState = LoaderState.ToUnload; break;
                                case LoaderState.Loading: reference.LoaderState = LoaderState.ToUnload; break;
                                case LoaderState.ToLoad: references.Remove(reference.Position); break;
                                case LoaderState.CancleUnload: reference.LoaderState = LoaderState.Unloading; break;
                            }

                        }
                    }
                }
            }
        }

        private void LoaderLoop()
        {
            while (true)
            {
                PlanetIndex2? toload = null;

                lock (lockObject)
                {
                    //Ermittelt das Element, welches als nächstes geladen werden soll
                    var subscriptionInfo = references.Values.Where(s => s.LoaderState == LoaderState.ToLoad).FirstOrDefault();

                    if (subscriptionInfo != null)
                    {
                        toload = subscriptionInfo.Position;
                        subscriptionInfo.LoaderState = LoaderState.Loading;
                    }
                }

                if (toload.HasValue)
                {

                    //TODO: Load toload
                    //Wenn es was zu laden gibt
                    var loadedEntities = resourceManager.LoadEntities(toload.Value.Planet, toload.Value.ColumnIndex);
                    entities.AddRange(loadedEntities);

                    lock (lockObject)
                    {
                        references[toload.Value].LoaderState = LoaderState.Ready;
                    }
                }
                else
                {
                    //Wenn es nichts zu laden gibt
                    PlanetIndex2? tounload = null;

                    lock (lockObject)
                    {
                        var subscriptionInfo = references.Values.Where(s => s.LoaderState == LoaderState.ToUnload).FirstOrDefault();

                        if (subscriptionInfo != null)
                        {
                            tounload = subscriptionInfo.Position;
                            subscriptionInfo.LoaderState = LoaderState.Unloading;
                        }
                    }

                    if (tounload.HasValue)
                    {
                        //Wenn es was zu entladen gibt
                        //TODO: Save
                        var savedEntities = entities.Where(e => e.Position.Planet == tounload.Value.Planet &&  e.Position.ChunkIndex.X == tounload.Value.ColumnIndex.X && e.Position.ChunkIndex.Y == tounload.Value.ColumnIndex.Y);
                        resourceManager.SaveEntities(tounload.Value.Planet, tounload.Value.ColumnIndex, savedEntities.ToArray());

                        lock (lockObject)
                        {
                            var subscriptionInfo = references[tounload.Value];

                            if (subscriptionInfo.LoaderState == LoaderState.CancleUnload)
                            {
                                subscriptionInfo.LoaderState = LoaderState.Ready;
                            }
                            else
                            {
                                references.Remove(subscriptionInfo.Position);
                                //TODO: Delete Entities
                            }
                        }
                    }
                }
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

        private class SubscriptionInfo
        {
            public PlanetIndex2 Position { get; set; }

            public int HardReference { get; set; }

            public int SoftReference { get; set; }

            public LoaderState LoaderState { get; set; }
        }

        private enum LoaderState
        {
            ToLoad, Loading, Ready, ToUnload, Unloading, CancleUnload
        }
    }
}
