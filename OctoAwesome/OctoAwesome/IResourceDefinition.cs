using System;
using System.Drawing;

namespace OctoAwesome
{
    public interface IResourceDefinition
    {
        string Name { get; }

        Bitmap Icon { get; }

        IResource GetInstance { get; }

        Type GetResourceType();
    }
}
