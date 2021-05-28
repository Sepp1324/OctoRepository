using System;
using System.Runtime.InteropServices;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal readonly struct VertexPositionNormalTextureLight : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormalTextureLight() => VertexDeclaration = new VertexDeclaration(sizeof(uint) * 2, new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.Position, 0), new VertexElement(sizeof(uint), VertexElementFormat.Single, VertexElementUsage.Normal, 0));

        public VertexPositionNormalTextureLight(Vector3 position, Vector3 normal, Vector2 uv, byte layer, uint light)
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
            PackedValue = (posX & 0xFF) | ((posY & 0xFF) << 8) | ((posZ & 0xFF) << 16) | ((uint) layer << 24);
            PackedValue2 = light | (normalPacked << 24) | (uvExpanded << 28);
        }

        public VertexPositionNormalTextureLight(uint packedValue1, uint packedValue2)
        {
            PackedValue = packedValue1;
            PackedValue2 = packedValue2;
        }

        public uint PackedValue { get; }
        
        public uint PackedValue2 { get; }
        
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}