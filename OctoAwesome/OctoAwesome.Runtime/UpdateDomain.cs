using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Runtime
{
    internal class UpdateDomain
    {
        private IUniverse universe;

        private Stopwatch watch;
        private Thread thread;

        public List<ActorHost> ActorHosts { get; set; }

        public bool Running { get; set; }

        public UpdateDomain(Stopwatch watch)
        {
            this.watch = watch;
            ActorHosts = new List<ActorHost>();

            var host = new ActorHost(new Player());

            ActorHosts.Add(host);

            Running = true;

            thread = new Thread(updateLoop);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        private void updateLoop()
        {
            TimeSpan lastCall = new TimeSpan();
            TimeSpan frameTime = new TimeSpan(0, 0, 0, 0, 16);

            while(Running)
            {
                //GameTime gameTime = new GameTime(watch.Elapsed, watch.Elapsed - lastCall);
                GameTime gameTime = new GameTime(watch.Elapsed, frameTime);

                lastCall = watch.Elapsed;

                //TODO: Chunk Updates

                //foreach (var actorHost in ActorHosts)
                //    actorHost.Update(gameTime);

                foreach (var actorHost in ActorHosts)
                    actorHost.Update(gameTime);

                if (watch.Elapsed - lastCall < frameTime)
                {
                    Thread.Sleep(frameTime - (watch.Elapsed - lastCall));
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            //foreach (var actorHost in ActorHosts)
            //    actorHost.Update(gameTime);
        }
    }
}