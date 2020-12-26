﻿using engenious;
using engenious.Graphics;
using System;

namespace OctoAwesome.Client.Components
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    struct VertexPositionNormalTextureLight : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormalTextureLight() => VertexDeclaration = new VertexDeclaration(sizeof(uint) * 2, new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.Position, 0), new VertexElement(sizeof(uint), VertexElementFormat.Single, VertexElementUsage.Normal, 0));

        public VertexPositionNormalTextureLight(Vector3 position, Vector3 normal, Vector2 uv, byte layer, uint light)
        {
            var posX = (uint)position.X;
            var posY = (uint)position.Y;
            var posZ = (uint)position.Z;

            var normalX = (int)normal.X;
            var normalY = (int)normal.Y;
            var normalZ = (int)normal.Z;

            var normalExpanded = (normalX + 1) * 100 + (normalY + 1) * 10 + (normalZ + 1);

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

            var uvExpanded = ((uint)uv.X << 1) | ((uint)uv.Y);
            PackedValue = (posX & 0xFF) | ((posY & 0xFF) << 8) | ((posZ & 0xFF) << 16) | ((uint)layer << 24);
            PackedValue2 = light | (normalPacked << 24) | (uvExpanded << 28);
        }

        public uint PackedValue { get; private set; }

        public uint PackedValue2 { get; private set; }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}