﻿using System;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Components
{
    internal struct VertexPositionNormalTexturePacked : IVertexType
    {
        //uv:(0,0),(0,1),(1,0),(1,1)
        //normal:(1,0,0),(-1,0,0)
        //      (0,1,0),(0,-1,0)
        //      (0,0,1),(0,0,-1)

        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormalTexturePacked()
        {
            VertexDeclaration = new VertexDeclaration(sizeof(uint) * 2,
                new VertexElement(0, VertexElementFormat.Rgba32, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(uint), VertexElementFormat.Rgba32, VertexElementUsage.Normal, 0));
        }

        public VertexPositionNormalTexturePacked(Vector3 position, Vector3 normal, Vector2 uv)
        {
            var posX = (uint)position.X;
            var posY = (uint)position.Y;
            var posZ = (uint)position.Z;

            var normalX = (int)normal.X;
            var normalY = (int)normal.Y;
            var normalZ = (int)normal.Z;

            var normalExpanded = (normalX + 1) * 100 + (normalY + 1) * 10 + normalZ + 1;

            uint normalPacked;
            switch (normalExpanded)
            {
                case 211:
                    normalPacked = 0;
                    break;
                case 11:
                    normalPacked = 1;
                    break;
                case 121:
                    normalPacked = 2;
                    break;
                case 101:
                    normalPacked = 3;
                    break;
                case 112:
                    normalPacked = 4;
                    break;
                case 110:
                    normalPacked = 5;
                    break;
                default:
                    throw new Exception("Expected error happened.");
            }

            var uvExpanded = ((uint)uv.X << 1) | (uint)uv.Y;

            PackedValue = (posX & 0xFF) | ((posY & 0xFF) << 8) | ((posZ & 0xFF) << 16) | (normalPacked << 24) |
                          (uvExpanded << 28);
            PackedValue2 = ((uint)(uv.X * 65536) << 16) | (uint)(uv.Y * 65536);
        }

        public uint PackedValue { get; }
        public uint PackedValue2 { get; }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}