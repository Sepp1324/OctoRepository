﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace OctoAwesome.Noise
{
    public class SimplexNoiseGenerator : INoise
    {
        public SimplexNoiseGenerator(int seed, float frequencyX = 1f, float frequencyY = 1f, float frequencyZ = 1f,
            float frequencyW = 1f)
        {
            Seed = seed;
            Octaves = 5;
            Persistance = 0.5f;
            FrequencyX = frequencyX;
            FrequencyY = frequencyY;
            FrequencyZ = frequencyZ;
            FrequencyW = frequencyW;
            Factor = 1;
            CreatePermutations();
        }

        #region Props & Fields

        private byte[] permutations;

        private static readonly byte[] range =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
            29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55,
            56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82,
            83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107,
            108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128,
            129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
            150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170,
            171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,
            192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
            213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233,
            234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255
        };

        private int octaves;
        private float persistance;

        public int Seed { get; }

        public float FrequencyX { get; set; }
        public float FrequencyY { get; set; }
        public float FrequencyZ { get; set; }
        public float FrequencyW { get; set; }

        public float Factor { get; set; }

        public int Octaves
        {
            get => octaves;
            set
            {
                octaves = value;
                RecalcMax();
            }
        }

        public float Persistance
        {
            get => persistance;
            set
            {
                persistance = value;
                RecalcMax();
            }
        }

        private void CreatePermutations()
        {
            var rnd = new Random(Seed);
            var temp = range.OrderBy(a => rnd.Next()).ToArray();
            permutations = temp.Concat(temp).ToArray();
        }

        public float MaxValue { get; private set; }

        private void RecalcMax()
        {
            MaxValue = 0f;
            for (var i = 0; i < Octaves; i++) MaxValue += (float)Math.Pow(Persistance, i);
        }

        #endregion

        #region NoiseMaps

        /// <summary>
        ///     Gibt ein float-Array einer 1D-Noise im angegebenem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition, ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Anzahl der gewollten Noise-Werte</param>
        /// <returns>Gibt ein float-Array einer 1D Noise zurück</returns>
        public float[] GetNoiseMap(int startX, int width)
        {
            var noise = new float[width];
            for (var x = 0; x < width; x++)
            {
                var frequencyX = FrequencyX;
                var amplitude = 1f;
                for (var i = 0; i < Octaves; i++)
                {
                    noise[x] += Noise((x + startX) * frequencyX) * amplitude;
                    amplitude *= Persistance;
                    frequencyX *= 2f;
                }

                noise[x] = noise[x] * Factor / MaxValue;
            }

            return noise;
        }

        /// <summary>
        ///     Gibt ein 2D-float-Array einer 2D-Noise im angegebem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <returns>Gibt ein 2D-float-Array einer 2D-Noise zurück</returns>
        public float[,] GetNoiseMap2D(int startX, int startY, int width, int height)
        {
            var noise = new float[width, height];

            Parallel.For(0, width, x =>
            {
                for (var y = 0; y < height; y++)
                {
                    var frequencyX = FrequencyX;
                    var frequencyZ = FrequencyZ;
                    var amplitude = 1f;
                    for (var i = 0; i < Octaves; i++)
                    {
                        noise[x, y] += Noise2D((x + startX) * frequencyX, (y + startY) * frequencyZ) * amplitude;
                        amplitude *= Persistance;
                        frequencyX *= 2f;
                        frequencyZ *= 2f;
                    }

                    noise[x, y] = noise[x, y] * Factor / MaxValue;
                }
            });
            return noise;
        }

        /// <summary>
        ///     Gibt ein 2D-float-Array einer 2D-Noise im angegebem Bereich zurück, welche kachelbar ist
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="sizeX">Breite der Noise-Map</param>
        /// <param name="sizeY">Höhe der Noise-Map</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein 2D-float-Array einer 2D-Noise zurück, welche kachelbar ist</returns>
        public float[] GetTileableNoiseMap2D(int startX, int startY, int sizeX, int sizeY, int tileSizeX, int tileSizeY,
            float[] array)
        {
            //float[,] noise = new float[sizeX, sizeY];

            Parallel.For(0, sizeX, x =>
            {
                for (var y = 0; y < sizeY; y++)
                {
                    var frequencyX = FrequencyX;
                    var frequencyY = FrequencyY;
                    float amplitude = 1;

                    var u = (float)(x + startX) / tileSizeX;
                    var v = (float)(y + startY) / tileSizeY;

                    var nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                    var ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
                    var nz = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                    var nw = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

                    for (var i = 0; i < Octaves; i++)
                    {
                        array[y * Chunk.CHUNKSIZE_X + x] +=
                            Noise4D(nx * frequencyX, ny * frequencyY, nz * frequencyX, nw * frequencyY) * amplitude;

                        amplitude *= Persistance;
                        frequencyX *= 2f;
                        frequencyY *= 2f;
                    }

                    array[y * Chunk.CHUNKSIZE_X + x] = array[y * Chunk.CHUNKSIZE_X + x] * Factor / MaxValue;
                }
            });

            return array;
        }

        /// <summary>
        ///     Gibt ein 3D-float-Array einer 3D-Noise im angegebem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startZ">Startposition auf der Z-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <param name="depth">Tiefe der Noise-Map</param>
        /// <returns>Gibt ein 3D-float-Array einer 3D-Noise zurück</returns>
        public float[,,] GetNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth)
        {
            var noise = new float[width, height, depth];


            Parallel.For(0, width, x =>
                //for (int x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                for (var z = 0; z < depth; z++)
                {
                    var frequencyX = FrequencyX;
                    var frequencyY = FrequencyY;
                    var frequencyZ = FrequencyZ;
                    var amplitude = 1f;
                    for (var i = 0; i < Octaves; i++)
                    {
                        noise[x, y, z] += Noise3D((x + startX) * frequencyX, (y + startY) * frequencyY,
                            (z + startZ) * frequencyZ) * amplitude;
                        amplitude *= Persistance;
                        frequencyX *= 2;
                        frequencyY *= 2;
                        frequencyZ *= 2;
                    }

                    noise[x, y, z] = noise[x, y, z] * Factor / MaxValue;
                }
            });
            return noise;
        }

        /// <summary>
        ///     Gibt ein 3D-float-Array einer 3D-Noise im angegebem Bereich zurück, welche in X und Y Richtung kachelbar ist
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startZ">Startposition auf der Z-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <param name="depth">Tiefe der Noise-Map</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein 3D-float-Array einer 3D-Noise zurück, welche in X und Y Richtung kachelbar ist</returns>
        public float[,,] GetTileableNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth,
            int tileSizeX, int tileSizeY)
        {
            var noise = new float[width, height, depth];

            Parallel.For(0, width, x =>
                //for (int x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                for (var z = 0; z < depth; z++)
                {
                    var frequencyX = FrequencyX;
                    var frequencyY = FrequencyY;
                    var frequencyZ = FrequencyZ;
                    float amplitude = 1;

                    var u = (float)(x + startX) / tileSizeX;
                    var v = (float)(y + startY) / tileSizeY;

                    var nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                    var ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
                    var nw = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                    var nv = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

                    for (var i = 0; i < Octaves; i++)
                    {
                        noise[x, y, z] += Noise5D(nx * frequencyX, ny * frequencyY, z * frequencyZ, nw * frequencyX,
                            nv * frequencyY) * amplitude;

                        amplitude *= Persistance;
                        frequencyX *= 2f;
                        frequencyY *= 2f;
                        frequencyZ *= 2f;
                    }

                    noise[x, y, z] = noise[x, y, z] * Factor / MaxValue;
                }
            });

            return noise;
        }

        /// <summary>
        ///     Gibt ein 4D-float-Array einer 4D-Noise im angegebem Bereich zurück
        /// </summary>
        /// <param name="startX">Startposition auf der X-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startY">Startposition auf der Y-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startZ">Startposition auf der Z-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="startW">Startposition auf der W-Achse,ab welcher die Noise Werte ausgegeben werden</param>
        /// <param name="width">Breite der Noise-Map</param>
        /// <param name="height">Höhe der Noise-Map</param>
        /// <param name="depth">Tiefe der Noise-Map</param>
        /// <param name="wDepth">Dicke(Tiefe 2.Grades) der Noise-Map</param>
        /// <returns>Gibt ein 4D-float-Array einer 4D-Noise zurück</returns>
        public float[,,,] GetNoiseMap4D(int startX, int startY, int startZ, int startW, int width, int height,
            int depth, int wDepth)
        {
            var noise = new float[width, height, depth, wDepth];

            Parallel.For(0, width, x =>
                //for (int x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                for (var z = 0; z < depth; z++)
                for (var w = 0; w < wDepth; w++)
                {
                    var frequencyX = FrequencyX;
                    var frequencyY = FrequencyY;
                    var frequencyZ = FrequencyZ;
                    var frequencyW = FrequencyW;
                    var amplitude = 1f;
                    for (var i = 0; i < Octaves; i++)
                    {
                        noise[x, y, z, w] += Noise4D((x + startX) * frequencyX, (y + startY) * frequencyY,
                            (z + startZ) * frequencyZ, (w + startW) * frequencyW) * amplitude;
                        amplitude *= Persistance;
                        frequencyX *= 2;
                        frequencyY *= 2;
                        frequencyZ *= 2;
                        frequencyW *= 2;
                    }

                    noise[x, y, z, w] = noise[x, y, z, w] * Factor / MaxValue;
                }
            });
            return noise;
        }

        #endregion

        #region SingleNoise

        /// <summary>
        ///     Gibt ein float-Wert einer 1D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 1D Noise zurück</returns>
        public float GetNoise(int x)
        {
            float noise = 0;
            var frequencyX = FrequencyX;
            var amplitude = 1f;
            for (var i = 0; i < Octaves; i++)
            {
                noise += Noise(x * frequencyX) * amplitude;
                amplitude *= Persistance;
                frequencyX *= 2;
            }

            return noise * Factor / MaxValue;
        }

        /// <summary>
        ///     Gibt ein float-Wert einer 2D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 2D Noise zurück</returns>
        public float GetNoise2D(int x, int y)
        {
            float noise = 0;
            var frequencyX = FrequencyX;
            var frequencyY = FrequencyY;
            float amplitude = 1;
            for (var i = 0; i < Octaves; i++)
            {
                noise += Noise2D(x * frequencyX, y * frequencyY) * amplitude;
                amplitude *= Persistance;
                frequencyX *= 2;
                frequencyY *= 2;
            }

            return noise * Factor / MaxValue;
        }

        /// <summary>
        ///     Gibt ein float-Wert einer 2D-Noise an gegebener Position zurück, welche kachelbar ist
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein float-Wert einer 2D Noise zurück, welche kachelbar ist</returns>
        public float GetTileableNoise2D(int x, int y, int tileSizeX, int tileSizeY)
        {
            float noise = 0;
            var frequencyX = FrequencyX;
            var frequencyY = FrequencyY;
            float amplitude = 1;

            var u = (float)x / tileSizeX;
            var v = (float)y / tileSizeY;

            var nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            var ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
            var nz = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            var nw = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

            for (var i = 0; i < Octaves; i++)
            {
                noise += Noise4D(nx * frequencyX, ny * frequencyY, nz * frequencyX, nw * frequencyY) * amplitude;

                amplitude *= Persistance;
                frequencyX *= 2f;
                frequencyY *= 2f;
            }

            return noise * Factor / MaxValue;
        }

        /// <summary>
        ///     Gibt ein float-Wert einer 3D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="z">Z-Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 3D Noise zurück</returns>
        public float GetNoise3D(int x, int y, int z)
        {
            float noise = 0;
            var frequencyX = FrequencyX;
            var frequencyY = FrequencyY;
            var frequencyZ = FrequencyZ;
            float amplitude = 1;

            for (var i = 0; i < Octaves; i++)
            {
                noise += Noise3D(x * frequencyX, y * frequencyY, z * frequencyZ) * amplitude;

                amplitude *= Persistance;
                frequencyX *= 2f;
                frequencyY *= 2f;
                frequencyZ *= 2f;
            }

            return noise * Factor / MaxValue;
        }

        /// <summary>
        ///     Gibt ein float-Wert einer 3D-Noise an gegebener Position zurück, welche in X und Y Richtung kachelbar ist
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="z">Z-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="tileSizeX">Breite der Kachel</param>
        /// <param name="tileSizeY">Höhe der Kachel</param>
        /// <returns>Gibt ein float-Wert einer 3D Noise zurück, welche in X und Y Richtung kachelbar ist</returns>
        public float GetTileableNoise3D(int x, int y, int z, int tileSizeX, int tileSizeY)
        {
            float noise = 0;
            var frequencyX = FrequencyX;
            var frequencyY = FrequencyY;
            var frequencyZ = FrequencyZ;
            float amplitude = 1;

            var u = (float)x / tileSizeX;
            var v = (float)y / tileSizeY;

            var nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            var ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
            var nw = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            var nv = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

            for (var i = 0; i < Octaves; i++)
            {
                noise += Noise5D(nx * frequencyX, ny * frequencyY, z * frequencyZ, nw * frequencyX, nv * frequencyY) *
                         amplitude;

                amplitude *= Persistance;
                frequencyX *= 2f;
                frequencyY *= 2f;
                frequencyZ *= 2f;
            }

            return noise * Factor / MaxValue;
        }

        /// <summary>
        ///     Gibt ein float-Wert einer 4D-Noise an gegebener Position zurück
        /// </summary>
        /// <param name="x">X-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="y">Y-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="z">Z-Position, für welche die Noise ausgegeben wird</param>
        /// <param name="w">W-Position, für welche die Noise ausgegeben wird</param>
        /// <returns>Gibt ein float-Wert einer 4D Noise zurück</returns>
        public float GetNoise4D(int x, int y, int z, int w)
        {
            float noise = 0;
            var frequencyX = FrequencyX;
            var frequencyY = FrequencyY;
            var frequencyZ = FrequencyZ;
            var frequencyW = FrequencyW;
            float amplitude = 1;

            for (var i = 0; i < Octaves; i++)
            {
                noise += Noise4D(x * frequencyX, y * frequencyY, z * frequencyZ, w * frequencyW) * amplitude;

                amplitude *= Persistance;
                frequencyX *= 2f;
                frequencyY *= 2f;
                frequencyZ *= 2f;
                frequencyW *= 2f;
            }

            return noise * Factor / MaxValue;
        }

        #endregion

        #region Noise Implementation

        private static readonly int[] grad3 =
        {
            1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0, 1, 0, 1, -1, 0, 1, 1, 0, -1, -1, 0, -1, 0, 1, 1, 0, -1, 1, 0, 1, -1,
            0, -1, -1
        };

        private static readonly int[] grad4 =
        {
            0, 1, 1, 1, 0, 1, 1, -1, 0, 1, -1, 1, 0, 1, -1, -1, 0, -1, 1, 1, 0, -1, 1, -1, 0, -1, -1, 1, 0, -1, -1, -1,
            1, 0, 1, 1, 1, 0, 1, -1, 1, 0, -1, 1, 1, 0, -1, -1, -1, 0, 1, 1, -1, 0, 1, -1, -1, 0, -1, 1, -1, 0, -1, -1,
            1, 1, 0, 1, 1, 1, 0, -1, 1, -1, 0, 1, 1, -1, 0, -1, -1, 1, 0, 1, -1, 1, 0, -1, -1, -1, 0, 1, -1, -1, 0, -1,
            1, 1, 1, 0, 1, 1, -1, 0, 1, -1, 1, 0, 1, -1, -1, 0, -1, 1, 1, 0, -1, 1, -1, 0, -1, -1, 1, 0, -1, -1, -1, 0
        };

        private static readonly int[] grad5 =
        {
            0, -1, -1, -1, -1, 0, -1, -1, -1, 1, 0, -1, -1, 1, -1, 0, -1, -1, 1, 1, 0, -1, 1, -1, -1, 0, -1, 1, -1, 1,
            0, -1, 1, 1, -1, 0, -1, 1, 1, 1, 0, 1, -1, -1, -1, 0, 1, -1, -1, 1, 0, 1, -1, 1, -1, 0, 1, -1, 1, 1, 0, 1,
            1, -1, -1, 0, 1, 1, -1, 1, 0, 1, 1, 1, -1, 0, 1, 1, 1, 1, -1, 0, -1, -1, -1, -1, 0, -1, -1, 1, -1, 0, -1, 1,
            -1, -1, 0, -1, 1, 1, -1, 0, 1, -1, -1, -1, 0, 1, -1, 1, -1, 0, 1, 1, -1, -1, 0, 1, 1, 1, 1, 0, -1, -1, -1,
            1, 0, -1, -1, 1, 1, 0, -1, 1, -1, 1, 0, -1, 1, 1, 1, 0, 1, -1, -1, 1, 0, 1, -1, 1, 1, 0, 1, 1, -1, 1, 0, 1,
            1, 1, -1, -1, 0, -1, -1, -1, -1, 0, -1, 1, -1, -1, 0, 1, -1, -1, -1, 0, 1, 1, -1, 1, 0, -1, -1, -1, 1, 0,
            -1, 1, -1, 1, 0, 1, -1, -1, 1, 0, 1, 1, 1, -1, 0, -1, -1, 1, -1, 0, -1, 1, 1, -1, 0, 1, -1, 1, -1, 0, 1, 1,
            1, 1, 0, -1, -1, 1, 1, 0, -1, 1, 1, 1, 0, 1, -1, 1, 1, 0, 1, 1, -1, -1, -1, 0, -1, -1, -1, -1, 0, 1, -1, -1,
            1, 0, -1, -1, -1, 1, 0, 1, -1, 1, -1, 0, -1, -1, 1, -1, 0, 1, -1, 1, 1, 0, -1, -1, 1, 1, 0, 1, 1, -1, -1, 0,
            -1, 1, -1, -1, 0, 1, 1, -1, 1, 0, -1, 1, -1, 1, 0, 1, 1, 1, -1, 0, -1, 1, 1, -1, 0, 1, 1, 1, 1, 0, -1, 1, 1,
            1, 0, 1, -1, -1, -1, -1, 0, -1, -1, -1, 1, 0, -1, -1, 1, -1, 0, -1, -1, 1, 1, 0, -1, 1, -1, -1, 0, -1, 1,
            -1, 1, 0, -1, 1, 1, -1, 0, -1, 1, 1, 1, 0, 1, -1, -1, -1, 0, 1, -1, -1, 1, 0, 1, -1, 1, -1, 0, 1, -1, 1, 1,
            0, 1, 1, -1, -1, 0, 1, 1, -1, 1, 0, 1, 1, 1, -1, 0, 1, 1, 1, 1, 0
        };

        private static float Dotproduct(int dIndex, float x, float y)
        {
            return grad3[dIndex] * x + grad3[dIndex + 1] * y;
        }

        private static float Dotproduct(int dIndex, float x, float y, float z)
        {
            return grad3[dIndex] * x + grad3[dIndex + 1] * y + grad3[dIndex + 2] * z;
        }

        private static float Dotproduct(int dIndex, float x, float y, float z, float w)
        {
            return grad4[dIndex] * x + grad4[dIndex + 1] * y + grad4[dIndex + 2] * z + grad4[dIndex + 3] * w;
        }

        private static float Dotproduct(int dIndex, float x, float y, float z, float w, float v)
        {
            return grad5[dIndex] * x + grad5[dIndex + 1] * y + grad5[dIndex + 2] * z + grad5[dIndex + 3] * w +
                   grad5[dIndex + 4] * v;
        }

        private static readonly float F2 = (float)(0.5f * (Math.Sqrt(3.0f) - 1.0f));
        private static readonly float G2 = (float)(3.0f - Math.Sqrt(3.0f)) / 6.0f;
        private static readonly float F3 = 1.0f / 3.0f;
        private static readonly float G3 = 1.0f / 6.0f;
        private static readonly float F4 = (float)(Math.Sqrt(5.0) - 1.0) / 4.0f;
        private static readonly float G4 = (float)(5.0 - Math.Sqrt(5.0)) / 20.0f;
        private static readonly float F5 = (float)((Math.Sqrt(6.0) - 1) / 5.0);
        private static readonly float G5 = (float)((6.0 - Math.Sqrt(6.0)) / 30.0);

        public static int Fastfloor(float val)
        {
            return val > 0 ? (int)val : (int)val - 1;
        }

        private float NoiseFunction(int x)
        {
            unchecked
            {
                var n = x * Seed;
                n = (n << 13) ^ n;
                n *= n * 15731;
                n += 789221;
                n *= n;
                n += 1376312589;
                n = n & 0x7fffffff;

                return (float)(1.0 - n / 1073741824.0);
            }
        }

        private float LinearInterpolation(float a, float b, float x)
        {
            return a + (b - a) * x;
        }

        private float Noise(float x)
        {
            var integer_X = (int)x;
            var fractional_X = x - integer_X;

            var v1 = NoiseFunction(integer_X);
            var v2 = NoiseFunction(integer_X + 1);


            return LinearInterpolation(v1, v2, fractional_X);
        }

        private float Noise2D(float xin, float yin)
        {
            float n0, n1, n2;

            var s = (xin + yin) * F2;

            var i = Fastfloor(xin + s);
            var j = Fastfloor(yin + s);

            var t = (i + j) * G2;
            var X0 = i - t;

            var Y0 = j - t;
            var x0 = xin - X0;

            var y0 = yin - Y0;

            int i1, j1;

            if (x0 > y0)
            {
                i1 = 1;
                j1 = 0;
            }

            else
            {
                i1 = 0;
                j1 = 1;
            }

            var x1 = x0 - i1 + G2;

            var y1 = y0 - j1 + G2;
            var x2 = x0 - 1.0f + 2.0f * G2;

            var y2 = y0 - 1.0f + 2.0f * G2;

            var ii = i & 255;
            var jj = j & 255;
            var gi0 = permutations[ii + permutations[jj]] % 12;
            var gi1 = permutations[ii + i1 + permutations[jj + j1]] % 12;
            var gi2 = permutations[ii + 1 + permutations[jj + 1]] % 12;

            var t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 <= 0)
            {
                n0 = 0.0f;
            }
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Dotproduct(gi0 * 3, x0, y0);
            }

            var t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 <= 0)
            {
                n1 = 0.0f;
            }
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Dotproduct(gi1 * 3, x1, y1);
            }

            var t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 <= 0)
            {
                n2 = 0.0f;
            }
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Dotproduct(gi2 * 3, x2, y2);
            }

            return 70.0f * (n0 + n1 + n2);
        }

        private float Noise3D(float x, float y, float z)
        {
            float n0, n1, n2, n3;
            var s = (x + y + z) * F3;
            var i = Fastfloor(x + s);
            var j = Fastfloor(y + s);
            var k = Fastfloor(z + s);
            var t = (i + j + k) * G3;
            var X0 = i - t;
            var Y0 = j - t;
            var Z0 = k - t;
            var x0 = x - X0;
            var y0 = y - Y0;
            var z0 = z - Z0;

            int i1, j1, k1;
            int i2, j2, k2;
            if (x0 >= y0)
            {
                if (y0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
                else if (x0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
            }
            else
            {
                if (y0 < z0)
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else if (x0 < z0)
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
            }

            var x1 = x0 - i1 + G3;
            var y1 = y0 - j1 + G3;
            var z1 = z0 - k1 + G3;
            var x2 = x0 - i2 + 2.0f * G3;
            var y2 = y0 - j2 + 2.0f * G3;
            var z2 = z0 - k2 + 2.0f * G3;
            var x3 = x0 - 1.0f + 3.0f * G3;
            var y3 = y0 - 1.0f + 3.0f * G3;
            var z3 = z0 - 1.0f + 3.0f * G3;

            var ii = i & 255;
            var jj = j & 255;
            var kk = k & 255;

            var gi0 = permutations[ii + permutations[jj + permutations[kk]]] % 12;
            var gi1 = permutations[ii + i1 + permutations[jj + j1 + permutations[kk + k1]]] % 12;
            var gi2 = permutations[ii + i2 + permutations[jj + j2 + permutations[kk + k2]]] % 12;
            var gi3 = permutations[ii + 1 + permutations[jj + 1 + permutations[kk + 1]]] % 12;

            var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 <= 0)
            {
                n0 = 0.0f;
            }
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Dotproduct(gi0 * 3, x0, y0, z0);
            }

            var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 <= 0)
            {
                n1 = 0.0f;
            }
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Dotproduct(gi1 * 3, x1, y1, z1);
            }

            var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 <= 0)
            {
                n2 = 0.0f;
            }
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Dotproduct(gi2 * 3, x2, y2, z2);
            }

            var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 <= 0)
            {
                n3 = 0.0f;
            }
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * Dotproduct(gi3 * 3, x3, y3, z3);
            }

            return 32.0f * (n0 + n1 + n2 + n3);
        }

        private float Noise4D(float x, float y, float z, float w)
        {
            float n0, n1, n2, n3, n4;

            var s = (x + y + z + w) * F4;
            var i = Fastfloor(x + s);
            var j = Fastfloor(y + s);
            var k = Fastfloor(z + s);
            var l = Fastfloor(w + s);
            var t = (i + j + k + l) * G4;
            var X0 = i - t;
            var Y0 = j - t;
            var Z0 = k - t;
            var W0 = l - t;
            var x0 = x - X0;
            var y0 = y - Y0;
            var z0 = z - Z0;
            var w0 = w - W0;

            var rankx = 0;
            var ranky = 0;
            var rankz = 0;
            var rankw = 0;
            if (x0 > y0) rankx++;
            else ranky++;
            if (x0 > z0) rankx++;
            else rankz++;
            if (x0 > w0) rankx++;
            else rankw++;
            if (y0 > z0) ranky++;
            else rankz++;
            if (y0 > w0) ranky++;
            else rankw++;
            if (z0 > w0) rankz++;
            else rankw++;
            int i1, j1, k1, l1;
            int i2, j2, k2, l2;
            int i3, j3, k3, l3;

            i1 = rankx >= 3 ? 1 : 0;
            j1 = ranky >= 3 ? 1 : 0;
            k1 = rankz >= 3 ? 1 : 0;
            l1 = rankw >= 3 ? 1 : 0;

            i2 = rankx >= 2 ? 1 : 0;
            j2 = ranky >= 2 ? 1 : 0;
            k2 = rankz >= 2 ? 1 : 0;
            l2 = rankw >= 2 ? 1 : 0;

            i3 = rankx >= 1 ? 1 : 0;
            j3 = ranky >= 1 ? 1 : 0;
            k3 = rankz >= 1 ? 1 : 0;
            l3 = rankw >= 1 ? 1 : 0;

            var x1 = x0 - i1 + G4;
            var y1 = y0 - j1 + G4;
            var z1 = z0 - k1 + G4;
            var w1 = w0 - l1 + G4;
            var x2 = x0 - i2 + 2.0f * G4;
            var y2 = y0 - j2 + 2.0f * G4;
            var z2 = z0 - k2 + 2.0f * G4;
            var w2 = w0 - l2 + 2.0f * G4;
            var x3 = x0 - i3 + 3.0f * G4;
            var y3 = y0 - j3 + 3.0f * G4;
            var z3 = z0 - k3 + 3.0f * G4;
            var w3 = w0 - l3 + 3.0f * G4;
            var x4 = x0 - 1.0f + 4.0f * G4;
            var y4 = y0 - 1.0f + 4.0f * G4;
            var z4 = z0 - 1.0f + 4.0f * G4;
            var w4 = w0 - 1.0f + 4.0f * G4;

            var ii = i & 255;
            var jj = j & 255;
            var kk = k & 255;
            var ll = l & 255;
            var gi0 = permutations[ii + permutations[jj + permutations[kk + permutations[ll]]]] % 32;
            var gi1 = permutations[ii + i1 + permutations[jj + j1 + permutations[kk + k1 + permutations[ll + l1]]]] %
                      32;
            var gi2 = permutations[ii + i2 + permutations[jj + j2 + permutations[kk + k2 + permutations[ll + l2]]]] %
                      32;
            var gi3 = permutations[ii + i3 + permutations[jj + j3 + permutations[kk + k3 + permutations[ll + l3]]]] %
                      32;
            var gi4 = permutations[ii + 1 + permutations[jj + 1 + permutations[kk + 1 + permutations[ll + 1]]]] % 32;

            var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 < 0)
            {
                n0 = 0.0f;
            }
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Dotproduct(gi0 * 4, x0, y0, z0, w0);
            }

            var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0)
            {
                n1 = 0.0f;
            }
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Dotproduct(gi1 * 4, x1, y1, z1, w1);
            }

            var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0)
            {
                n2 = 0.0f;
            }
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Dotproduct(gi2 * 4, x2, y2, z2, w2);
            }

            var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0)
            {
                n3 = 0.0f;
            }
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * Dotproduct(gi3 * 4, x3, y3, z3, w3);
            }

            var t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0)
            {
                n4 = 0.0f;
            }
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * Dotproduct(gi4 * 4, x4, y4, z4, w4);
            }

            return 27.0f * (n0 + n1 + n2 + n3 + n4);
        }

        private float Noise5D(float x, float y, float z, float w, float v)
        {
            float n0, n1, n2, n3, n4, n5;

            var s = (x + y + z + w + v) * F5;
            var i = Fastfloor(x + s);
            var j = Fastfloor(y + s);
            var k = Fastfloor(z + s);
            var l = Fastfloor(w + s);
            var m = Fastfloor(v + s);
            var t = (i + j + k + l + m) * G5;
            var X0 = i - t;
            var Y0 = j - t;
            var Z0 = k - t;
            var W0 = l - t;
            var V0 = m - t;
            var x0 = x - X0;
            var y0 = y - Y0;
            var z0 = z - Z0;
            var w0 = w - W0;
            var v0 = v - V0;

            var rankx = 0;
            var ranky = 0;
            var rankz = 0;
            var rankw = 0;
            var rankv = 0;
            if (x0 > y0) rankx++;
            else ranky++;
            if (x0 > z0) rankx++;
            else rankz++;
            if (x0 > w0) rankx++;
            else rankw++;
            if (x0 > v0) rankx++;
            else rankv++;

            if (y0 > z0) ranky++;
            else rankz++;
            if (y0 > w0) ranky++;
            else rankw++;
            if (y0 > v0) ranky++;
            else rankv++;

            if (z0 > w0) rankz++;
            else rankw++;
            if (z0 > v0) rankz++;
            else rankv++;

            int i1, j1, k1, l1, m1;
            int i2, j2, k2, l2, m2;
            int i3, j3, k3, l3, m3;
            int i4, j4, k4, l4, m4;

            i1 = rankx >= 4 ? 1 : 0;
            j1 = ranky >= 4 ? 1 : 0;
            k1 = rankz >= 4 ? 1 : 0;
            l1 = rankw >= 4 ? 1 : 0;
            m1 = rankv >= 4 ? 1 : 0;

            i2 = rankx >= 3 ? 1 : 0;
            j2 = ranky >= 3 ? 1 : 0;
            k2 = rankz >= 3 ? 1 : 0;
            l2 = rankw >= 3 ? 1 : 0;
            m2 = rankv >= 3 ? 1 : 0;

            i3 = rankx >= 2 ? 1 : 0;
            j3 = ranky >= 2 ? 1 : 0;
            k3 = rankz >= 2 ? 1 : 0;
            l3 = rankw >= 2 ? 1 : 0;
            m3 = rankv >= 2 ? 1 : 0;

            i4 = rankx >= 1 ? 1 : 0;
            j4 = ranky >= 1 ? 1 : 0;
            k4 = rankz >= 1 ? 1 : 0;
            l4 = rankw >= 1 ? 1 : 0;
            m4 = rankv >= 1 ? 1 : 0;

            var x1 = x0 - i1 + G5;
            var y1 = y0 - j1 + G5;
            var z1 = z0 - k1 + G5;
            var w1 = w0 - l1 + G5;
            var v1 = v0 - m1 + G5;

            var x2 = x0 - i2 + 2.0f * G5;
            var y2 = y0 - j2 + 2.0f * G5;
            var z2 = z0 - k2 + 2.0f * G5;
            var w2 = w0 - l2 + 2.0f * G5;
            var v2 = v0 - m2 + 2.0f * G5;

            var x3 = x0 - i3 + 3.0f * G5;
            var y3 = y0 - j3 + 3.0f * G5;
            var z3 = z0 - k3 + 3.0f * G5;
            var w3 = w0 - l3 + 3.0f * G5;
            var v3 = v0 - m3 + 3.0f * G5;

            var x4 = x0 - i4 + 4.0f * G5;
            var y4 = y0 - j4 + 4.0f * G5;
            var z4 = z0 - k4 + 4.0f * G5;
            var w4 = w0 - l4 + 4.0f * G5;
            var v4 = v0 - m4 + 4.0f * G5;

            var x5 = x0 - 1.0f + 5.0f * G5;
            var y5 = y0 - 1.0f + 5.0f * G5;
            var z5 = z0 - 1.0f + 5.0f * G5;
            var w5 = w0 - 1.0f + 5.0f * G5;
            var v5 = v0 - 1.0f + 5.0f * G5;

            var ii = i & 255;
            var jj = j & 255;
            var kk = k & 255;
            var ll = l & 255;
            var mm = m & 255;
            var gi0 = permutations[ii + permutations[jj + permutations[kk + permutations[ll + permutations[mm]]]]] % 80;
            var gi1 = permutations[
                ii + i1 + permutations[
                    jj + j1 + permutations[kk + k1 + permutations[ll + l1 + permutations[mm + m1]]]]] % 80;
            var gi2 = permutations[
                ii + i2 + permutations[
                    jj + j2 + permutations[kk + k2 + permutations[ll + l2 + permutations[mm + m2]]]]] % 80;
            var gi3 = permutations[
                ii + i3 + permutations[
                    jj + j3 + permutations[kk + k3 + permutations[ll + l3 + permutations[mm + m3]]]]] % 80;
            var gi4 = permutations[
                ii + i4 + permutations[
                    jj + j4 + permutations[kk + k4 + permutations[ll + l4 + permutations[mm + m4]]]]] % 80;
            var gi5 = permutations[
                          ii + 1 + permutations[
                              jj + 1 + permutations[kk + 1 + permutations[ll + 1 + permutations[mm + 1]]]]] %
                      80;

            var t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0 - v0 * v0;
            if (t0 < 0)
            {
                n0 = 0.0f;
            }
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Dotproduct(gi0 * 5, x0, y0, z0, w0, v0);
            }

            var t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1 - v1 * v1;
            if (t1 < 0)
            {
                n1 = 0.0f;
            }
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Dotproduct(gi1 * 5, x1, y1, z1, w1, v1);
            }

            var t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2 - v2 * v2;
            if (t2 < 0)
            {
                n2 = 0.0f;
            }
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Dotproduct(gi2 * 5, x2, y2, z2, w2, v2);
            }

            var t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3 - v3 * v3;
            if (t3 < 0)
            {
                n3 = 0.0f;
            }
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * Dotproduct(gi3 * 5, x3, y3, z3, w3, v3);
            }

            var t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4 - v4 * v4;
            if (t4 < 0)
            {
                n4 = 0.0f;
            }
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * Dotproduct(gi4 * 5, x4, y4, z4, w4, v4);
            }

            var t5 = 0.6f - x5 * x5 - y5 * y5 - z5 * z5 - w5 * w5 - v5 * v5;
            if (t5 < 0)
            {
                n5 = 0.0f;
            }
            else
            {
                t5 *= t5;
                n5 = t5 * t5 * Dotproduct(gi5 * 5, x5, y5, z5, w5, v5);
            }

            return 18.0f * (n0 + n1 + n2 + n3 + n4 + n5);
        }

        #endregion

        #region Sonstiges

        //List<int[]> grads = new List<int[]>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        for (int x = -1; x <= 1; x += 2)
        //        {
        //            for (int y = -1; y <= 1; y += 2)
        //            {
        //                for (int z = -1; z <= 1; z += 2)
        //                {
        //                    for (int w = -1; w <= 1; w += 2)
        //                    {
        //                        for (int v = -1; v <= 1; v += 2)
        //                        {
        //                            int[] grad = new int[] { x, y, z, w, v };
        //                            grad[i] = 0;

        //                            grads.Add(grad);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    grads = grads.Distinct(new comp()).ToList();

        //    Debug.Write("{");
        //    foreach (int[] grad in grads)
        //        Debug.Write(String.Join(",", grad) + ",");

        //class comp : EqualityComparer<int[]>
        //{

        //    public override bool Equals(int[] x, int[] y)
        //    {
        //        if (x == null || y == null)
        //            return false;
        //        if (x.Length != y.Length)
        //            return false;
        //        for (int i = 0; i < x.Length; i++)
        //        {
        //            if (x[i] != y[i])
        //                return false;
        //        }
        //        return true;
        //    }

        //    public override int GetHashCode(int[] obj)
        //    {
        //        return obj[0];
        //    }
        //}

        //float min = float.MaxValue, max = float.MinValue;
        //    SimplexNoiseGenerator gen = new SimplexNoiseGenerator(0);

        //    for (int x = -10; x <= 10; x ++)
        //    {
        //        for (int y = -10; y <= 10; y ++)
        //        {
        //            for (int z = -10; z <= 10; z ++)
        //            {
        //                for (int w = -10; w <= 10; w ++)
        //                {
        //                    for (int v = -10; v <= 10; v ++)
        //                    {
        //                        float val = gen.Noise5D(x, y, z, w);

        //                        min = Math.Min(val, min);
        //                        max = Math.Max(val, max);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    min = 1f / min;
        //    max = 1f / max;

        #endregion
    }
}