namespace OctoAwesome.Definitions.Items
{
    public class Hand : Item
    {
        public Hand(HandDefinition handDefinition) : base(handDefinition, null)
        {

        }

        public override int Hit(IMaterialDefinition material, decimal blockVolumeVolumeRemaining, int volumePerHit) => volumePerHit / 5;
    }
}
