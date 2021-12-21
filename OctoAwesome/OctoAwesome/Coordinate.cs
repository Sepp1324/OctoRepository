using System;
using System.Xml.Serialization;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    ///     Datenstruktur zur genauen Position von Spiel-Elementen innerhalb der OctoAwesome Welt.
    /// </summary>
    public struct Coordinate
    {
        /// <summary>
        ///     Index des Planeten im Universum.
        /// </summary>
        public int Planet;

        /// <summary>
        ///     Index des betroffenen Blocks.
        /// </summary>
        private Index3 _block;

        /// <summary>
        ///     Position innerhalb des Blocks (0...1).
        /// </summary>
        private Vector3 _position;

        /// <summary>
        ///     Erzeugt eine neue Instanz der Coordinate-Struktur.
        /// </summary>
        /// <param name="planet">Index des Planeten</param>
        /// <param name="block">Blockindex innerhalb des Planeten</param>
        /// <param name="position">Position innerhalb des Blockes</param>
        public Coordinate(int planet, Index3 block, Vector3 position)
        {
            Planet = planet;
            _block = block;
            _position = position;
            Normalize();
        }

        /// <summary>
        ///     Gibt den Index des Chunks zurück oder legt diesen fest.
        /// </summary>
        [XmlIgnore]
        public Index3 ChunkIndex
        {
            get => new(_block.X >> Chunk.LimitX, _block.Y >> Chunk.LimitY, _block.Z >> Chunk.LimitZ);
            set
            {
                var localBlockIndex = LocalBlockIndex;
                _block = new(value.X * Chunk.CHUNKSIZE_X + localBlockIndex.X, value.Y * Chunk.CHUNKSIZE_Y + localBlockIndex.Y, value.Z * Chunk.CHUNKSIZE_Z + localBlockIndex.Z);
            }
        }

        /// <summary>
        ///     Gibt den globalen Index (Planet-Koordinaten) des Blockes zurück oder legt diesen fest.
        /// </summary>
        public Index3 GlobalBlockIndex
        {
            get => _block;
            set => _block = value;
        }

        /// <summary>
        ///     Gibt den lokalen Index des Blocks (Chunk-Koordinaten) zurück oder legt diesen fest.
        /// </summary>
        [XmlIgnore]
        public Index3 LocalBlockIndex
        {
            get
            {
                var chunk = ChunkIndex;
                return new(_block.X - chunk.X * Chunk.CHUNKSIZE_X, _block.Y - chunk.Y * Chunk.CHUNKSIZE_Y, _block.Z - chunk.Z * Chunk.CHUNKSIZE_Z);
            }
            set
            {
                var chunk = ChunkIndex;
                GlobalBlockIndex = new(chunk.X * Chunk.CHUNKSIZE_X + value.X, chunk.Y * Chunk.CHUNKSIZE_Y + value.Y, chunk.Z * Chunk.CHUNKSIZE_Z + value.Z);
                Normalize();
            }
        }

        /// <summary>
        ///     Gibt die globale Position (Planet-Koordinaten) als Vektor zurück oder legt diesen fest.
        /// </summary>
        [XmlIgnore]
        public Vector3 GlobalPosition
        {
            get => new(_block.X + _position.X, _block.Y + _position.Y, _block.Z + _position.Z);
            set
            {
                _block = Index3.Zero;
                _position = value;
                Normalize();
            }
        }

        /// <summary>
        ///     Gibt die lokale Position (Chunk-Koordinaten) als Vektor zurück oder legt diese fest.
        /// </summary>
        [XmlIgnore]
        public Vector3 LocalPosition
        {
            get
            {
                var blockIndex = LocalBlockIndex;
                return new(blockIndex.X + _position.X, blockIndex.Y + _position.Y, blockIndex.Z + _position.Z);
            }
            set
            {
                var chunkIndex = ChunkIndex;
                _block = new(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y, chunkIndex.Z * Chunk.CHUNKSIZE_Z);
                _position = value;
                Normalize();
            }
        }

        /// <summary>
        ///     Gibt die Position innerhalb des aktuellen Blockes zurück oder legt diese fest.
        /// </summary>
        public Vector3 BlockPosition
        {
            get => _position;
            set
            {
                _position = value;
                Normalize();
            }
        }

        /// <summary>
        ///     Normalisiert die vorhandenen Parameter auf den Position-Wertebereich von [0...1] und die damit verbundene
        ///     Verschiebung im Block.
        /// </summary>
        private void Normalize()
        {
            var shift = new Index3((int)Math.Floor(_position.X), (int)Math.Floor(_position.Y), (int)Math.Floor(_position.Z));

            _block += shift;
            _position -= shift;
        }

        /// <summary>
        ///     Normalisiert den ChunkIndex auf die gegebenen Limits.
        /// </summary>
        /// <param name="limit"></param>
        public void NormalizeChunkIndexXY(Index3 limit)
        {
            var index = ChunkIndex;
            index.NormalizeXY(limit);
            ChunkIndex = index;
        }

        /// <summary>
        ///     Addiert die zwei gegebenen <see cref="Coordinate" />s.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <exception cref="NotSupportedException">Wenn die beiden Coordinates nicht auf den selben Planeten verweisen</exception>
        /// <returns>Das Ergebnis der Addition</returns>
        public static Coordinate operator +(Coordinate i1, Coordinate i2)
        {
            if (i1.Planet != i2.Planet)
                throw new NotSupportedException();

            return new(i1.Planet, i1._block + i2._block, i1._position + i2._position);
        }

        /// <summary>
        ///     Addiert den gegebenen Vector3 auf die <see cref="BlockPosition" /> der Coordinate.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns>Das Ergebnis der Addition</returns>
        public static Coordinate operator +(Coordinate i1, Vector3 i2) => new(i1.Planet, i1._block, i1._position + i2);

        /// <summary>
        ///     Stellt die Coordinate-Instanz als string dar.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $@"({Planet}/{(_block.X + _position.X):0.000000}/{(_block.Y + _position.Y):0.000000}/{(_block.Z + _position.Z):0.000000})";

        /// <summary>
        ///     Compare this object with an other object
        /// </summary>
        /// <param name="obj">a other object</param>
        /// <returns>true if both objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Coordinate coordinate)
                return base.Equals(obj) || Planet == coordinate.Planet && _position == coordinate._position && _block == coordinate._block;

            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => base.GetHashCode();
    }
}