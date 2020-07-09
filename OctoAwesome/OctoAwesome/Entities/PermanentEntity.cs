using System.IO;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Entität, das dauert haft simuliert werden muss (z.B Player)
    /// </summary>
    public class PermanentEntity : ControllableEntity
    {
        /// <summary>
        /// Aktivierungsradius
        /// </summary>
        public readonly int ActivationRange;

        public PermanentEntity(int activationRange)
        {
            ActivationRange = activationRange;
        }
    }
}
