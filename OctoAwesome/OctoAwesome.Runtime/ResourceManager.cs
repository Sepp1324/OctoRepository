using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ResourceManager
    {
        #region Singleton

        private static ResourceManager instance = null;

        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceManager();
                return instance;
            }
        }

        #endregion

        private ResourceManager()
        {

        }

        public IPlanet GetPlanet(int id)
        {
            throw new NotImplementedException();
        }
    }
}
