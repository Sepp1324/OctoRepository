using engenious;

namespace OctoAwesome.EntityComponents
{
    public class ControllableComponent : EntityComponent
    {
        public ControllableComponent()
        {
        }

        public bool JumpInput { get; set; }
        public Vector2 MoveInput { get; set; }

        public bool JumpActive { get; set; }
        public int JumpTime { get; set; }

        public Index3? InteractBlock { get; set; }
        public Index3? ApplyBlock { get; set; }
        public OrientationFlags ApplySide { get; set; }
    }
}