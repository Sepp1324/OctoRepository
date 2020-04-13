using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public struct Coordinate
    {
        public Index3 Block;

        public Vector3 Position;

        public Coordinate(Index3 block, Vector3 position)
        {
            Block = block;
            Position = position;
        }

        public Vector3 AsVector3()
        {
            return new Vector3(Block.X + Position.X, Block.Y + Position.Y, Block.Z + Position.Z);
        }

        public static Coordinate operator +(Coordinate i1, Coordinate i2)
        {
            Vector3 position = i1.Position + i2.Position;
            Index3 block = i1.Block + i2.Block;

            block.X += (int)Math.Floor(position.X);
            position.X = Math.Abs(position.X) % 1;

            block.Y += (int)Math.Floor(position.Y);
            position.Y = Math.Abs(position.Y) % 1;

            block.Z += (int)Math.Floor(position.Z);
            position.Z = Math.Abs(position.Z) % 1;

            return new Coordinate(block, position);
        }
        public static Coordinate operator +(Coordinate i1, Vector3 i2)
        {
            Vector3 position = i1.Position + i2;
            Index3 block = i1.Block;

            block.X += (int)Math.Floor(position.X);
            position.X = Math.Abs(position.X) % 1;

            block.Y += (int)Math.Floor(position.Y);
            position.Y = Math.Abs(position.Y) % 1;

            block.Z += (int)Math.Floor(position.Z);
            position.Z = Math.Abs(position.Z) % 1;

            return new Coordinate(block, position);
        }
    }
}
