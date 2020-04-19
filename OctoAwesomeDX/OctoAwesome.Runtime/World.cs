﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    public sealed class World
    {
        

        private UpdateDomain[] updateDomains;

        public Player Player { get { return updateDomains[0].Player; } }

        public World(IInputSet input, int planetCount, IMapGenerator mapGenerator, IChunkPersistence chunkPersistence)
        {
            updateDomains = new UpdateDomain[1];
            updateDomains[0] = new UpdateDomain(input, planetCount, mapGenerator, chunkPersistence);
        }

        public IPlanet GetPlanet(int id)
        {
            return updateDomains[0].GetPlanet(id);
        }

        public void Update(GameTime frameTime)
        {
            foreach (var updateDomain in updateDomains)
            {
                updateDomain.Update(frameTime);
            }
        }

        public void Save()
        {
            foreach (var updateDomain in updateDomains)
            {
                updateDomain.Save();
            }
        }
    }
}