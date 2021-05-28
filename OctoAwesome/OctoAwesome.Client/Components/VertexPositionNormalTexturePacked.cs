using System;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Components
{
    internal readonly struct VertexPositionNormalTexturePacked : IVertexType
    {
        //uv:(0,0),(0,1),(1,0),(1,1)
        //normal:(1,0,0),(-1,0,0)
        //      (0,1,0),(0,-1,0)
        //      (0,0,1),(0,0,-1)

        private static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormalTexturePacked() => VertexDeclaration = new VertexDeclaration(sizeof(uint) * 2, new VertexElement(0, VertexElementFormat.Rgba32, VertexElementUsage.Position, 0), new VertexElement(sizeof(uint), VertexElementFormat.Rgba32, VertexElementUsage.Normal, 0));

        public VertexPositionNormalTexturePacked(Vector3 position, Vector3 normal, Vector2 uv)
        {
            var posX = (uint) position.X;
            var posY = (uint) position.Y;
            var posZ = (uint) position.Z;

            var normalX = (int) normal.X;
            var normalY = (int) normal.Y;
            var normalZ = (int) normal.Z;

            var normalExpanded = (normalX + 1) * 100 + (normalY + 1) * 10 + normalZ + 1;

            uint normalPacked = normalExpanded switch
            {
                211 => 0,
                11 => 1,
                121 => 2,
                101 => 3,
                112 => 4,
                110 => 5,
                _ => throw new Exception("Expected error happened.")
            };

            var uvExpanded = ((uint) uv.X << 1) | (uint) uv.Y;

            PackedValue = (posX & 0xFF) | ((posY & 0xFF) << 8) | ((posZ & 0xFF) << 16) | (normalPacked << 24) | (uvExpanded << 28);
            PackedValue2 = ((uint) (uv.X * 65536) << 16) | (uint) (uv.Y * 65536);
        }

        private uint PackedValue { get; }
        private uint PackedValue2 { get; }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}