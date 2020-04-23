using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OctoAwesome.Runtime
{
    internal class UpdateDomain
    {
        private IUniverse universe;

        public List<ActorHost> ActorHosts { get; set; }

        public UpdateDomain(IInputSet input)
        {
            ActorHosts = new List<ActorHost>();

            var host = new ActorHost(new Player(), input);

            ActorHosts.Add(host);
        }

        public void Update(GameTime frameTime)
        {
           foreach(var actorHost in ActorHosts)
            {
                actorHost.Update(frameTime);
            }
        }

        public void Save()
        {
            ResourceManager.Instance.Save();
        }
    }
}