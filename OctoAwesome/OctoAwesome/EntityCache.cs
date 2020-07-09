using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public sealed class EntityCache
    {
        private object lockObject = new object();

        private Dictionary<PlanetIndex2, SubscriptionInfo> references = new Dictionary<PlanetIndex2, SubscriptionInfo>();

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
                                case LoaderState.CancleUnload: reference.LoaderState = LoaderState.Unloading; break;
                                case LoaderState.ToUnload: reference.LoaderState = LoaderState.Ready; break;
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
                        loaderStates[toload.Value] = LoaderState.Loading;
                    }
                }

                if (toload.HasValue)
                {

                    //TODO: Load toload
                    //Wenn es was zu laden gibt
                    lock (lockObject)
                    {
                        loaderStates[toload.Value] = LoaderState.Ready;
                    }
                }
                else
                {
                    //Wenn es nichts zu laden gibt
                    PlanetIndex2? tounload = null;

                    lock (lockObject)
                    {
                        if (loaderStates.Where(s => s.Value == LoaderState.ToUnload).Any())
                        {
                            tounload = loaderStates.Where(s => s.Value == LoaderState.ToUnload).First().Key;
                            loaderStates[tounload.Value] = LoaderState.Unloading;
                        }
                    }

                    if (tounload.HasValue)
                    {
                        //Wenn es was zu entladen gibt
                        //TODO: Save

                        lock (lockObject)
                        {
                            if (loaderStates[tounload.Value] == LoaderState.CancleUnload)
                            {
                                loaderStates[tounload.Value] = LoaderState.Ready;
                            }
                            else
                            {
                                loaderStates.Remove(tounload.Value);
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
