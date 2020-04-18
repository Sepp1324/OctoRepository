using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    /// <summary>
    /// Struktur zur Definierung einer zweidimensionalen Index-Position
    /// </summary>
    public struct Index2
    {
        public int X;
        public int Y;

        public Index2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Index2(Index2 value) : this(value.X, value.Y) { }

        public Index2(Index3 value) : this(value.X, value.Y) { }

        public void NormalizeX(int size)
        {
            X = NormalizeAxis(X, size);
        }

        public void NormalizeX(Index2 size)
        {
            NormalizeX(size.X);
        }

        public void NormalizeX(Index3 size)
        {
            NormalizeX(size.X);
        }

        public void NormalizeY(int size)
        {
            Y = NormalizeAxis(Y, size);
        }

        public void NormalizeY(Index2 size)
        {
            NormalizeY(size.Y);
        }

        public void NormalizeZ(Index3 size)
        {
            NormalizeY(size.Y);
        }

        public void NormalizeXY(int x, int y)
        {
            NormalizeX(x);
            NormalizeY(y);
        }

        public void NormalizeXY(Index2 size)
        {
            NormalizeXY(size.X, size.Y);
        }

        public void NormalizeXY(Index3 size)
        {
            NormalizeX(size.X);
            NormalizeY(size.Y);
        }

        public void Normalize(int x, int y)
        {
            NormalizeX(x);
            NormalizeY(y);
        }

        public void Normalize(Index2 size)
        {
            Normalize(size.X, size.Y);
        }

        public void Normalize(Index3 size)
        {
            Normalize(size.X, size.Y);
        }

        public static int NormalizeAxis(int value, int size)
        {
            value %= size;
            if (value < 0) value += size;
            return value;
        }

        public int ShortestDistanceX(int x, int size)
        {
            return Index2.ShortestDistanceOnAxis(X, x, size);
        }

        public int ShortestDistanceY(int y, int size)
        {
            return Index2.ShortestDistanceOnAxis(Y, y, size);
        }

        public Index2 ShortestDistanceXY(Index2 destination, Index2 size)
        {
            return new Index2(ShortestDistanceX(destination.X, size.X), ShortestDistanceY(destination.Y, size.Y));
        }

        public static int ShortestDistanceOnAxis(int origin, int destination, int size)
        {
            origin = NormalizeAxis(origin, size);
            destination = NormalizeAxis(origin, size);
            int half = size / 2;

            int distance = destination - origin;

            if (distance > half) distance -= size;
            else if (distance < -half) distance += size;
            return distance;
        }

        public static Index2 operator +(Index2 i1, Index2 i2)
        {
            return new Index2(i1.X + i2.X, i1.Y + i2.Y);
        }

        public static Index2 operator *(Index2 i1, int scale)
        {
            return new Index2(i1.X * scale, i1.Y * scale);
        }

        public static Index2 operator /(Index2 i1, int scale)
        {
            return new Index2(i1.X / scale, i1.Y / scale);
        }

        public static bool operator ==(Index2 i1, Index2 i2)
        {
            return i1.Equals(i2);
        }

        public static bool operator !=(Index2 i1, Index2 i2)
        {
            return !i1.Equals(i2);
        }

        public static Index2 operator -(Index2 i1, Index2 i2)
        {
            return new Index2(i1.X - i2.X, i1.Y - i2.Y);
        }

        //public static int Normalize(int value, int size)
        //{
        //    value %= size;
        //    if (value < 0) value += size;
        //    return value;
        //}

        public override string ToString()
        {
            return "(" + X.ToString() + "/" + Y.ToString() + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Index2)) return false;

            Index2 other = (Index2)obj;
            return (other.X == this.X && other.Y == this.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }
    }
}
