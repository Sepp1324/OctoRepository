using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public struct Coordinate
    {
        public int Planet;

        private Index3 block;

        private Vector3 position;

        public Coordinate(int planet, Index3 block, Vector3 position)
        {
            Planet = planet;
            this.block = block;
            this.position = position;

            this.Normalize();
        }

        public Index3 ChunkIndex
        {
            get
            {
                return new Index3((int)(block.X / Chunk.CHUNKSIZE_X), (int)(block.Y / Chunk.CHUNKSIZE_Y), (int)(block.Z / Chunk.CHUNKSIZE_Z));
            }
            set
            {
                Vector3 localPosition = new Vector3(block.X % Chunk.CHUNKSIZE_X + position.X, block.Y % Chunk.CHUNKSIZE_Y + position.Y, block.Z % Chunk.CHUNKSIZE_Z + position.Z);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Index3 GlobalBlockIndex
        {
            get { return block; }
            set { block = value; }
        }

        public Index3 LocalBlockIndex
        {
            get { return new Index3(block.X % Chunk.CHUNKSIZE_X, block.Y % Chunk.CHUNKSIZE_Y, block.Z % Chunk.CHUNKSIZE_Z); }
            set { throw new NotImplementedException(); }
        }

        public Vector3 GlobalPosition
        {
            get { return new Vector3(block.X + position.X, block.Y + position.Y, block.Z + position.Z); }
            set
            {
                position = GlobalPosition;
                Normalize();
            }
        }

        public Vector3 LocalPosition
        {

        }

        public Vector3 AsVector3()
        {
            return new Vector3(block.X + position.X, block.Y + position.Y, block.Z + position.Z);
        }

        public Index3 AsChunk()
        {
            return new Index3((int)(block.X / Chunk.CHUNKSIZE_X), (int)(block.Y / Chunk.CHUNKSIZE_Y), (int)(block.Z / Chunk.CHUNKSIZE_Z));
        }

        public Index3 AsLocalBlock()
        {
            return new Index3(block.X % Chunk.CHUNKSIZE_X, block.Y % Chunk.CHUNKSIZE_Y, block.Z % Chunk.CHUNKSIZE_Z);
        }

        public Vector3 AsLocalPosition()
        {
            return new Vector3(block.X % Chunk.CHUNKSIZE_X + position.X, block.Y % Chunk.CHUNKSIZE_Y + position.Y, block.Z % Chunk.CHUNKSIZE_Z + position.Z);
        }

        public void Normalize()
        {
            block.X += (int)Math.Floor(position.X);
            position.X = (position.X >= 0) ? (position.X = position.X % 1) : (1f + (position.X % 1));

            block.Y += (int)Math.Floor(position.Y);
            position.Y = (position.Y >= 0) ? (position.Y = position.Y % 1) : (1f + (position.Y % 1));

            block.Z += (int)Math.Floor(position.Z);
            position.Z = (position.Z >= 0) ? (position.Z = position.Z % 1) : (1f + (position.Z % 1));
        }

        public static Coordinate operator +(Coordinate i1, Coordinate i2)
        {
            Vector3 position = i1.position + i2.position;
            Index3 block = i1.block + i2.block;

            if (i1.Planet != i2.Planet)
                throw new NotSupportedException();

            Coordinate result = new Coordinate(i1.Planet, block, position);
            result.Normalize();
            return result;
        }

        public static Coordinate operator +(Coordinate i1, Vector3 i2)
        {
            Vector3 position = i1.position + i2;
            Index3 block = i1.block;

            Coordinate result = new Coordinate(i1.Planet, block, position);
            result.Normalize();
            return result;
        }

        public override string ToString()
        {
            return "(" + Planet + "/" + (block.X + position.X).ToString("0.00") + "/" + (block.Y + position.Y).ToString("0.00") + "/" + (block.Z + position.Z).ToString("0.00") + ")";
        }
    }
}
