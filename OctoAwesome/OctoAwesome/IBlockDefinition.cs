using System;
using System.Collections.Generic;
using System.Drawing;

namespace OctoAwesome
{
    public interface IBlockDefinition
    {
        string Name { get; }

        IEnumerable<Bitmap> Textures { get; }

        int GetTopTextureIndex(IBlock block);

        int GetBottomTextureIndex(IBlock block);

        int GetNorthTextureIndex(IBlock block);

        int GetSouthTextureIndex(IBlock block);

        int GetWestTextureIndex(IBlock block);

        int GetEastTextureIndex(IBlock block);

        int GetTopTextureRotation(IBlock block);

        int GetBottomTextureRotation(IBlock block);

        int GetNorthTextureRotation(IBlock block);

        int GetSouthTextureRotation(IBlock block);

        int GetWestTextureRotation(IBlock block);

        int GetEastTextureRotation(IBlock block);

        IBlock GetInstance(OrientationFlags orientation);

        Type GetBlockType();
    }
}