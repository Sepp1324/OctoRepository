﻿using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public sealed class GrassBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Grass"; }
        }

        public Bitmap Icon
        {
            get { return (Bitmap)Bitmap.FromFile("./Assets/grass_top.png"); }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {


                return new[] {
                    (Bitmap)Bitmap.FromFile("./Assets/grass_top.png"),
                    (Bitmap)Bitmap.FromFile("./Assets/dirt.png"),
                    (Bitmap)Bitmap.FromFile("./Assets/dirt_grass.png"),
                };
            }
        }

        public PhysicalProperties GetProperties(IBlock block)
        {
            return new PhysicalProperties()
            {
                Density = 0.3f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public void Hit(IBlock block, PhysicalProperties itemProperties)
        {
            block.Condition -= 10;
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new GrassBlock();
        }

        public Type GetBlockType()
        {
            return typeof(GrassBlock);
        }


        public int GetTopTextureIndex(IBlock block)
        {
            return 0;
        }

        public int GetBottomTextureIndex(IBlock block)
        {
            return 1;
        }

        public int GetNorthTextureIndex(IBlock block)
        {
            return 2;
        }

        public int GetSouthTextureIndex(IBlock block)
        {
            return 2;
        }

        public int GetWestTextureIndex(IBlock block)
        {
            return 2;
        }

        public int GetEastTextureIndex(IBlock block)
        {
            return 2;
        }

        public int GetTopTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetBottomTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetEastTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetWestTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetNorthTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetSouthTextureRotation(IBlock block)
        {
            return 0;
        }


        public bool IsTopSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsBottomSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsNorthSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsSouthSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsWestSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsEastSolidWall(IBlock block)
        {
            return true;
        }
    }
}