namespace OctoAwesome.Basics.Information
{
    public sealed class BlockCollectionInformation : BlockInteractionInformation
    {
        public decimal VolumesRemaining { get; set; }

        public void SetBlock(BlockInfo blockInfo, IBlockDefinition blockDefinition)
        {
            VolumesRemaining = blockDefinition.VolumePerUnit;
        }
    }
}