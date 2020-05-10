using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    [Flags]
    public enum Wall
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        Back,
        Front
    }
}
