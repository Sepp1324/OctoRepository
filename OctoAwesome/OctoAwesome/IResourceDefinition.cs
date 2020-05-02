using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IResourceDefinition
    {
        string Name { get; }

        Bitmap Icon { get; }

        IResource GetInstance();

        Type GetResourceType();
    }
}