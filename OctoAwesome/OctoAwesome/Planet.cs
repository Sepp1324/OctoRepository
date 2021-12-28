using System;
using System.IO;
using OctoAwesome.Notifications;

namespace OctoAwesome
{
    /// <summary>
    ///     Standard-Implementation of the Planet
    /// </summary>
    public class Planet : IPlanet
    {
        private bool _disposed;

        /// <summary>
        ///     Initialization of the Planet
        /// </summary>
        /// <param name="id">ID of the Planet</param>
        /// <param name="universe">ID of the Universe</param>
        /// <param name="size">Size of the Planet</param>
        /// <param name="seed">Seed of the Random-Generator</param>
        public Planet(int id, Guid universe, Index3 size, int seed) : this()
        {
            Id = id;
            Universe = universe;
            Size = new((int)Math.Pow(2, size.X), (int)Math.Pow(2, size.Y), (int)Math.Pow(2, size.Z));
            Seed = seed;
        }

        /// <summary>
        ///     Instantiate new Planet
        /// </summary>
        public Planet() => GlobalChunkCache = new GlobalChunkCache(this, TypeContainer.Get<IResourceManager>(), TypeContainer.Get<IUpdateHub>());

        /// <summary>
        ///     ID of the Planet
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        ///     Reference to the Parent-Universe
        /// </summary>
        public Guid Universe { get; private set; }

        /// <summary>
        ///     ClimateMap of the Planet
        /// </summary>
        public IClimateMap ClimateMap { get; protected set; }

        /// <summary>
        ///     Seed of the Random-Generator
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        ///     Size of the Planet in Chunks
        /// </summary>
        public Index3 Size { get; private set; }

        /// <summary>
        ///     Gravity of the Planet
        /// </summary>
        public float Gravity { get; protected set; }

        /// <summary>
        ///     Generator of the Planet
        /// </summary>
        public IMapGenerator Generator { get; set; }

        /// <summary>
        ///     GlobalChunkCache for the Planet
        /// </summary>
        public IGlobalChunkCache GlobalChunkCache { get; set; }

        /// <summary>
        ///     Serializes the Planet with the given <see cref="BinaryWriter" />
        /// </summary>
        /// <param name="writer">Given <see cref="BinaryWriter" /></param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Seed);
            writer.Write(Gravity);
            writer.Write(Size.X);
            writer.Write(Size.Y);
            writer.Write(Size.Z);
            writer.Write(Universe.ToByteArray());
        }

        /// <summary>
        ///     Deserializes the Planet with the given <see cref="BinaryReader" />
        /// </summary>
        /// <param name="reader">Given <see cref="BinaryReader" /></param>
        public virtual void Deserialize(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Seed = reader.ReadInt32();
            Gravity = reader.ReadSingle();
            Size = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            Universe = new(reader.ReadBytes(16));
            //var name = reader.ReadString();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (GlobalChunkCache is IDisposable disposable)
                disposable.Dispose();

            GlobalChunkCache = null;
        }
    }
}