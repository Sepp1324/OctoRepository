﻿using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    ///     Erzeugt ein lokales Koordinatensystem.
    /// </summary>
    public class LocalBuilder
    {
        private readonly IChunkColumn _column00, _column01, _column10, _column11;
        private readonly int _originX, _originY, _originZ;

        /// <summary>
        ///     Erzeugt eine neue Instanz der Klasse LocalBuilder
        /// </summary>
        /// <param name="originX"></param>
        /// <param name="originY"></param>
        /// <param name="originZ"></param>
        /// <param name="column00"></param>
        /// <param name="column10"></param>
        /// <param name="column01"></param>
        /// <param name="column11"></param>
        public LocalBuilder(int originX, int originY, int originZ, IChunkColumn column00, IChunkColumn column10,
            IChunkColumn column01, IChunkColumn column11)
        {
            _originX = originX;
            _originY = originY;
            _originZ = originZ;

            _column00 = column00;
            _column01 = column01;
            _column10 = column10;
            _column11 = column11;
        }

        /// <summary>
        /// </summary>
        /// <param name="column00"></param>
        /// <param name="column10"></param>
        /// <param name="column01"></param>
        /// <param name="column11"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static IChunkColumn GetColumn(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01,
            IChunkColumn column11, int x, int y)
        {
            var column = x switch
            {
                >= Chunk.CHUNKSIZE_X when y >= Chunk.CHUNKSIZE_Y => column11,
                < Chunk.CHUNKSIZE_X when y >= Chunk.CHUNKSIZE_Y => column01,
                >= Chunk.CHUNKSIZE_X when y < Chunk.CHUNKSIZE_Y => column10,
                _ => column00
            };

            return column;
        }

        /// <summary>
        /// </summary>
        /// <param name="column00"></param>
        /// <param name="column10"></param>
        /// <param name="column01"></param>
        /// <param name="column11"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int GetSurfaceHeight(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01,
            IChunkColumn column11, int x, int y)
        {
            var curColumn = GetColumn(column00, column10, column01, column11, x, y);
            return curColumn.Heights[x % Chunk.CHUNKSIZE_X, y % Chunk.CHUNKSIZE_Y];
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="block"></param>
        /// <param name="meta"></param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            x += _originX;
            y += _originY;
            z += _originZ;
            var column = GetColumn(_column00, _column10, _column01, _column11, x, y);
            var index = z / Chunk.CHUNKSIZE_Z;
            x %= Chunk.CHUNKSIZE_X;
            y %= Chunk.CHUNKSIZE_Y;
            z %= Chunk.CHUNKSIZE_Z;
            var flatIndex = Chunk.GetFlatIndex(x, y, z);
            column.Chunks[index].Blocks[flatIndex] = block;
        }

        /// <summary>
        /// </summary>
        /// <param name="issueNotification"></param>
        /// <param name="blockInfos"></param>
        public void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos)
        {
            blockInfos
                .Select(b =>
                {
                    var x = b.Position.X + _originX;
                    var y = b.Position.Y + _originY;
                    var z = b.Position.Z + _originZ;
                    var column = GetColumn(_column00, _column10, _column01, _column11, x, y);
                    var index = z / Chunk.CHUNKSIZE_Z;
                    x %= Chunk.CHUNKSIZE_X;
                    y %= Chunk.CHUNKSIZE_Y;
                    z %= Chunk.CHUNKSIZE_Z;
                    var info = new BlockInfo(x, y, z, b.Block, b.Meta);
                    return new { info, index, column };
                })
                .GroupBy(a => a.column)
                .ForEach(column => column
                    .GroupBy(i => i.index)
                    .ForEach(
                        i => column.Key.Chunks[i.Key].SetBlocks(issueNotification, i.Select(b => b.info).ToArray())));
        }


        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <param name="block"></param>
        /// <param name="meta"></param>
        public void FillSphere(int x, int y, int z, int radius, ushort block, int meta = 0)
        {
            var blockInfos = new List<BlockInfo>(radius * 6);

            for (var i = -radius; i <= radius; i++)
            for (var j = -radius; j <= radius; j++)
            for (var k = -radius; k <= radius; k++)
                if (i * i + j * j + k * k < radius * radius)
                    blockInfos.Add((x + i, y + j, z + k, block, meta));
            SetBlocks(false, blockInfos.ToArray());
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public ushort GetBlock(int x, int y, int z)
        {
            x += _originX;
            y += _originY;
            z += _originZ;
            var column = GetColumn(_column00, _column10, _column01, _column11, x, y);
            x %= Chunk.CHUNKSIZE_X;
            y %= Chunk.CHUNKSIZE_Y;
            return column.GetBlock(x, y, z);
        }
    }
}