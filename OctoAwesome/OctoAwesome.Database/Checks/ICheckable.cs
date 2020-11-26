using System.IO;

namespace OctoAwesome.Database.Checks
{
    public interface ICheckable<TTag> where TTag : ITag, new()
    {
        void Check();
    }
}