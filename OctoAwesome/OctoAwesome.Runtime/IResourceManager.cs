namespace OctoAwesome.Runtime
{
    public interface IResourceManager
    {
        IUniverse GetUniverse(int id);

        IPlanet GetPlanet(int id);

        IPlanetResourceManager Get(int planetId);
    }
}