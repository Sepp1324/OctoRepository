﻿using Microsoft.Xna.Framework;
using System;
using System.Drawing;

namespace OctoAwesome
{
    public abstract class BlockDefinition : IBlockDefinition
    {
        public abstract string Name { get; }

        public abstract Bitmap Icon { get; }

        public virtual int StackLimit { get { return 100; } }

        public abstract Bitmap[] Textures { get; }

        public virtual bool HasMetaData { get { return false; } }

        public abstract PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z);

        public abstract void Hit(IBlockDefinition block, PhysicalProperties itemProperties);

        public virtual BoundingBox[] GetCollisionBoxes(IPlanetResourceManager manager, int x, int y, int z)
        {
            return new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) };
        }

        public virtual int GetTopTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetBottomTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetNorthTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetSouthTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetWestTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetEastTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetTopTextureRotation(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetBottomTextureRotation(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetEastTextureRotation(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetWestTextureRotation(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetNorthTextureRotation(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetSouthTextureRotation(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual bool IsTopSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsBottomSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsNorthSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsSouthSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsWestSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsEastSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return true;
        }
    }

    public interface IUpdateable
    {
        void Tick(IPlanetResourceManager manager, int x, int y, int z);
    }
}