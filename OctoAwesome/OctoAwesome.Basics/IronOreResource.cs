using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class IronOreResource : IResourceDefinition
    {
        public string Name
        {
            get
            {
                return "Iron Ore";
            }
        }

        public Bitmap Icon => throw new NotImplementedException();

        public IResource GetInstance => throw new NotImplementedException();

        public Type GetResourceType()
        {
            throw new NotImplementedException();
        }
    }
}
