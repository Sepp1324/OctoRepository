﻿using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public class Chunk
    {
        public const int CHUNKSIZE_X = 32;
        public const int CHUNKSIZE_Y = 32;
        public const int CHUNKSIZE_Z = 32; 

        public IBlock[,,] Blocks { get; set; }

        public Chunk()
        {
            Blocks = new IBlock[CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z];

            for(int z = 0; z < CHUNKSIZE_Z; z++)
            {
                float heightZ = (float)Math.Sin((float)(z * Math.PI) / 16f);

                for(int y = 0; y < CHUNKSIZE_Y; y++)
                {
                    for(int x = 0; x < CHUNKSIZE_X; x++)
                    {
                        float heightX = (float)Math.Sin((float)(x * Math.PI) / 16f);
                        float height = (heightX + heightZ) * 2;

                        if (y < (int)(16 + height))
                        {
                            if (x % 2 == 0 || y % 2 == 0)
                                Blocks[x, y, z] = new GrassBlock();
                            else
                                Blocks[x, y, z] = new SandBlock();
                        }
                    }
                }
            }
        }
    }
}
