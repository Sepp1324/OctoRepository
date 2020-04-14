using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public interface IMapGenerator
    {
        Planet GeneratePlanet(int seed);

        Chunk GenerateChunk(Planet planet, Index3 index);
    }
}
