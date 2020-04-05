using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public interface IInputSet
    {
        bool Left { get; }
        bool Right { get; }
        bool Up { get; }
        bool Down { get; }

        bool HeadLeft { get; }
        bool HeadRight { get; }
        bool HeadUp { get; }
        bool HeadDown { get; }

        bool Interact { get; }
    }
}
