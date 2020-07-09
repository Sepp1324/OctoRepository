namespace OctoAwesome.Entities
{
    /// <summary>
    /// Entity, das dauert haft simuliert werden muss (z.B Player)
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
