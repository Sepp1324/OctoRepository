using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OctoAwesome.Model
{
    public struct Index3
    {
        public int X;
        public int Y;
        public int Z;

        public Index3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Index3(Index3 index, int z) : this(index.X, index.Y, z) { }

        public Index3(Index3 index) : this(index.X, index.Y, index.Z) { }

        public Index3(Index2 index, int z) : this(index.X, index.Y, z) {}

        public void NormalizeX(int size)
        {
            X = Index2.NormalizeAxis(X, size);
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
            Y = Index2.NormalizeAxis(Y, size);
        }

        public void NormalizeY(Index2 size)
        {
            NormalizeY(size.Y);
        }

        public void NormalizeY(Index3 size)
        {
            NormalizeY(size.Y);
        }

        public void NormalizeZ(int size)
        {
            Z = Index2.NormalizeAxis(Z, size);
        }

        public void NormalizeZ(Index3 size)
        {
            NormalizeZ(size.Z);
        }

        public void NormalizeXY(int x, int y)
        {
            NormalizeX(x);
            NormalizeY(y);
        }

        public void NormalizeXY(Index2 size)
        {
            NormalizeX(size.X);
            NormalizeY(size.Y);
        }

        public void NormalizeXY(Index3 size)
        {
            NormalizeXY(size.X, size.Y);
        }

        public void NormalizeXYZ(int x, int y, int z)
        {
            NormalizeX(x);
            NormalizeY(y);
            NormalizeZ(z);
        }

        public void NormalizeXYZ(Index3 size)
        {
            NormalizeXYZ(size.X, size.Y, size.Z);
        }

        public void NormalizeXYZ(Index2 size, int z)
        {
            NormalizeXYZ(size.X, size.Y, z);
        }

        public static Index3 operator +(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
        }

        public static Index3 operator -(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);
        }

        public static bool operator ==(Index3 i1, Index3 i2) {
            return i1.Equals(i2);
        }

        public static bool operator !=(Index3 i1, Index3 i2)
        {
            return !i1.Equals(i2);
        }

        public static Index3 operator *(Index3 i1, int scale)
        {
            return new Index3(i1.X * scale, i1.Y * scale, i1.Z * scale);
        }

        public static Index3 operator /(Index3 i1, int scale)
        {
            return new Index3(i1.X / scale, i1.Y / scale, i1.Z / scale);
        }

        public static Index3 operator -(Index3 i1, Index2 i2)
        {
            return new Index3(i1.X - i2.X, i1.Y - i2.Y-i2.Y, i1.Z);
        }

        public int ShortestDistanceX(int x, int size)
        {
            return Index2.ShortestDistanceOnAxis(X, x, size);
        }

        public int ShortestDistanceY(int y, int size)
        {
            return Index2.ShortestDistanceOnAxis(Y, y, size);
        }

        public int ShortestDistanceZ(int z, int size)
        {
            return Index2.ShortestDistanceOnAxis(Z, z, size);
        }

        public Index2 ShortestDistanceXY(Index2 destination, Index2 size)
        {
            return new Index2(ShortestDistanceX(destination.X, size.X), ShortestDistanceY(destination.Y, size.Y));
        }

        public Index3 ShortestDistanceXY(Index3 destination, Index3 size)
        {
            return new Index3(ShortestDistanceX(destination.X, size.X), ShortestDistanceY(destination.Y, size.Y), destination.Z - Z);
        }

        public Index3 ShortestDistanceXY(Index3 destination, Index2 size)
        {
            return new Index3(ShortestDistanceX(destination.X, size.X), ShortestDistanceY(destination.Y, size.Y), destination.Z - Z);
        }

        public Index3 ShortestDistanceXYZ(Index3 destination, Index3 size)
        {
            return new Index3(ShortestDistanceX(destination.X, size.X), ShortestDistanceY(destination.X, size.Y), ShortestDistanceZ(destination.Z, size.Z));
        }

        public override string ToString()
        {
            return "(" + X.ToString() + "/" + Y.ToString() + "/" + Z.ToString() + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Index3)) return false;

            Index3 other = (Index3)obj;
            return (other.X == this.X && other.Y == this.Y && other.Z == this.Z);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }
    }
}
