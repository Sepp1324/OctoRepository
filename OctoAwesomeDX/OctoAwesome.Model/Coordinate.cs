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

        public static Coordinate operator +(Coordinate c1, Coordinate c2)
        {
            Vector3 position = c1.Position + c2.Position;
            Index3 block = c1.Block + c2.Block;

            block.X += (int)Math.Floor(position.X);
            position.X = position.X % 1;

            block.Y += (int)Math.Floor(position.Y);
            position.Y = position.Y % 1;

            block.Z += (int)Math.Floor(position.Z);
            position.Z = position.Z % 1;

            return new Coordinate(block, position);
        }
        public static Coordinate operator +(Coordinate c, Vector3 v)
        {
            Vector3 position = c.Position + v;
            Index3 block = c.Block;

            block.X += (int)Math.Floor(position.X);
            position.X = position.X % 1;

            block.Y += (int)Math.Floor(position.Y);
            position.Y = position.Y % 1;

            block.Z += (int)Math.Floor(position.Z);
            position.Z = position.Z % 1;

            return new Coordinate(block, position);
        }
        public static Coordinate operator +(Vector3 v, Coordinate c)
        {
            Vector3 position = c.Position + v;
            Index3 block = c.Block;

            block.X += (int)Math.Floor(position.X);
            position.X = position.X % 1;

            block.Y += (int)Math.Floor(position.Y);
            position.Y = position.Y % 1;

            block.Z += (int)Math.Floor(position.Z);
            position.Z = position.Z % 1;

            return new Coordinate(block, position);
        }
    }
}
