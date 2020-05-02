using System.Collections.Generic;

namespace OctoAwesome
{
    public interface IMapGenerator
    {
        IUniverse GenerateUniverse(int id);

        IPlanet GeneratePlanet(int universe, int seed);

        IChunk[] GenerateChunk(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, Index2 index);
    }
}