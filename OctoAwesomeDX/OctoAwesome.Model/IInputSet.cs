using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public interface IInputSet
    {
        float MoveX { get; }
        float MoveY { get; }

        float HeadX { get; }
        float HeadY { get; }

        bool InteractTrigger { get; }
        bool JumpTrigger { get; }
    }
}
