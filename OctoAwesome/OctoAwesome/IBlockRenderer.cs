using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome
{
    public interface IBlockRenderer
    {
        VertexPositionNormalTexture[] GenerateMesh(IPlanetResourceManager manager, int x, int y, int z, 
            bool blockedTop, bool blockedBottom, bool blockedNorth, bool blockedSout, bool blockedWest, bool blockedEast);
    }
}
