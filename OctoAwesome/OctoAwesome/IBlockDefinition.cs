using System;
using System.Collections.Generic;
using System.Drawing;

namespace OctoAwesome
{
    public interface IBlockDefinition
    {
        string Name { get; }

        IEnumerable<Bitmap> Textures { get; }

        int GetTextureIndexTop(IBlock block);

        int GetTextureIndexBottom(IBlock block);

        int GetTextureIndexNorth(IBlock block);

        int GetTextureIndexSouth(IBlock block);

        int GetTextureIndexWest(IBlock block);

        int GetTextureIndexEast(IBlock block);

        int GetTextureRotationTop(IBlock block);

        int GetTextureRotationBottom(IBlock block);

        int GetTextureRotationNorth(IBlock block);

        int GetTextureRotationSouth(IBlock block);

        int GetTextureRotationWest(IBlock block);

        int GetTextureRotationEast(IBlock block);

        IBlock GetInstance(OrientationFlags orientation);

        Type GetBlockType();
    }
}