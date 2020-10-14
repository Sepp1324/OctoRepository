using System;

namespace OctoAwesome.Database
{
    public readonly struct IdTag : ITagable
    {
        public int Tag { get; }

        public IdTag(int id) => Tag = id;
    }
}