using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace OctoAwesome.Runtime
{
    public sealed class World
    {
        private Stopwatch watch = new Stopwatch();

        private UpdateDomain[] updateDomains;

        public ActorHost Player { get { return updateDomains[0].ActorHosts[0]; } }

        public World()
        {
            watch.Start();
            updateDomains = new UpdateDomain[1];
            updateDomains[0] = new UpdateDomain(watch);
        }

        public void Update(GameTime frameTime)
        {
            updateDomains[0].Update(frameTime);
        }

        public void Save()
        {
            updateDomains[0].Running = false;
            ResourceManager.Instance.Save();
        }
    }
}