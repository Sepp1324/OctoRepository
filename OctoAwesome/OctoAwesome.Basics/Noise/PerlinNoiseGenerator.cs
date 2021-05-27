using System;

namespace OctoAwesome.Noise
{
    public class PerlinNoiseGenerator : INoise
    {
        public float[,,,] GetNoiseMap4D(int startX, int startY, int startZ, int startW, int width, int height, int depth, int wDepth)
        {
            throw new NotImplementedException();
        }

        public float[] GetTileableNoiseMap2D(int startX, int startY, int width, int height, int tileSizeX, int tileSizeY, float[] noiseArray)
        {
            throw new NotImplementedException();
        }

        public float[,,] GetTileableNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth, int tileSizeX, int tileSizeY)
        {
            throw new NotImplementedException();
        }

        public float GetNoise(int x)
        {
            throw new NotImplementedException();
        }

        public float GetNoise2D(int x, int y)
        {
            throw new NotImplementedException();
        }

        public float GetTileableNoise2D(int x, int y, int tileSizeX, int tileSizeY)
        {
            throw new NotImplementedException();
        }

        public float GetNoise3D(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public float GetNoise4D(int x, int y, int z, int w)
        {
            throw new NotImplementedException();
        }


        public float GetTileableNoise3D(int x, int y, int z, int tileSizeX, int tileSizeY)
        {
            throw new NotImplementedException();
        }

        public float GetTileableNoise3D(int x, int y, int z, int tileSizeX, int tileSizeY, int tileSizeZ)
        {
            throw new NotImplementedException();
        }

        #region Interface

        public float[] GetNoiseMap(int startX, int width)
        {
            return PerlinNoise(startX, width);
        }

        public float[,] GetNoiseMap2D(int startX, int startY, int width, int heigth)
        {
            return PerlinNoise2(startX, startY, width, heigth);
        }

        public float[,,] GetNoiseMap3D(int startX, int startY, int startZ, int width, int heigth, int depth)
        {
            return PerlinNoise3(startX, startY, startZ, width, heigth, depth);
        }

        #endregion

        #region NoiseCode

        public float Smoothfactor { get; set; }
        public float Persistance { get; set; }
        public int Octaves { get; set; }
        public int Sizefactor { get; set; }
        public int Seed { get; }


        public PerlinNoiseGenerator(int seed, float smoothfactor = 0, float persistance = 0.25f, int octaves = 3, int sizefactor = 64)
        {
            Seed = seed;
            Smoothfactor = smoothfactor;
            Persistance = persistance;
            Octaves = octaves;
            Sizefactor = sizefactor;
        }


        #region Noise

        public float Noise(int x)
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

                return (float) (1.0 - n / 1073741824.0);
            }
        }

        public float Noise2(int x, int y)
        {
            unchecked
            {
                var n = x + y * 57 * Seed;
                n = (n << 13) ^ n;
                n *= n * 15731;
                n += 789221;
                n *= n;
                n += 1376312589;
                n = n & 0x7fffffff;

                return (float) (1.0 - n / 1073741824.0);
            }
        }

        public float Noise3(int x, int y, int z)
        {
            unchecked
            {
                var n = x + y * 29 + z * 37 * Seed;
                n = (n << 13) ^ n;
                n *= n * 15731;
                n += 789221;
                n *= n;
                n += 1376312589;
                n = n & 0x7fffffff;

                return (float) (1.0 - n / 1073741824.0);
            }
        }

        #endregion

        #region LinearInterpolation

        private float LinearInterpolation(float a, float b, float x)
        {
            return a * (1 - x) + b * x;
        }

        private float LinearInterpolation2(float a, float b, float c, float d, float x, float y)
        {
            var v1 = LinearInterpolation(a, b, x);
            var v2 = LinearInterpolation(c, d, x);
            return LinearInterpolation(v1, v2, y);
        }

        private float LinearInterpolation3(float a, float b, float c, float d, float e, float f, float g, float h, float x, float y, float z)
        {
            var v1 = LinearInterpolation2(a, b, c, d, x, y);
            var v2 = LinearInterpolation2(e, f, g, h, x, y);
            return LinearInterpolation(v1, v2, z);
        }

        #endregion

        #region InterpolatedNoise

        private float InterpolatedNoise(float x)
        {
            var integer_X = (int) x;
            var fractional_X = x - integer_X;

            var v1 = Noise(integer_X);
            var v2 = Noise(integer_X + 1);


            return LinearInterpolation(v1, v2, fractional_X);
        }

        private float InterpolatedNoise2(float x, float y)
        {
            var integer_X = (int) x;
            var fractional_X = x - integer_X;

            var integer_Y = (int) y;
            var fractional_Y = y - integer_Y;

            var v1 = Noise2(integer_X, integer_Y);
            var v2 = Noise2(integer_X + 1, integer_Y);
            var v3 = Noise2(integer_X, integer_Y + 1);
            var v4 = Noise2(integer_X + 1, integer_Y + 1);


            return LinearInterpolation2(v1, v2, v3, v4, fractional_X, fractional_Y);
        }

        private float InterpolatedNoise3(float x, float y, float z)
        {
            var integer_X = (int) x;
            var fractional_X = x - integer_X;

            var integer_Y = (int) y;
            var fractional_Y = y - integer_Y;

            var integer_Z = (int) z;
            var fractional_Z = z - integer_Z;

            var v1 = Noise3(integer_X, integer_Y, integer_Z);
            var v2 = Noise3(integer_X + 1, integer_Y, integer_Z);
            var v3 = Noise3(integer_X, integer_Y + 1, integer_Z);
            var v4 = Noise3(integer_X + 1, integer_Y + 1, integer_Z);

            var v5 = Noise3(integer_X, integer_Y, integer_Z + 1);
            var v6 = Noise3(integer_X + 1, integer_Y, integer_Z + 1);
            var v7 = Noise3(integer_X, integer_Y + 1, integer_Z + 1);
            var v8 = Noise3(integer_X + 1, integer_Y + 1, integer_Z + 1);

            return LinearInterpolation3(v1, v2, v3, v4, v5, v6, v7, v8, fractional_X, fractional_Y, fractional_Z);
        }

        #endregion

        #region SmoothedNoise

        public float SmoothedNoise(int x)
        {
            if (Smoothfactor == 0) return Noise(x);

            return Noise(x) * ((Noise(x + 1) - Noise(x - 1)) / Smoothfactor);
        }

        public float SmoothedNoise2(int x, int y)
        {
            if (Smoothfactor == 0) return Noise2(x, y);

            var sides = (Noise2(x + 1, y) + Noise2(x - 1, y) + Noise2(x, y + 1) + Noise2(x, y - 1)) / (4 * Smoothfactor);
            var corners = (Noise2(x + 1, y + 1) + Noise2(x - 1, y - 1) + Noise2(x - 1, y + 1) + Noise2(x + 1, y - 1)) / (4 * (float) Math.Sqrt(2) * Smoothfactor);
            var center = Noise2(x, y) / (2 * Smoothfactor);

            return center + sides + corners;
        }

        public float SmoothedNoise3(int x, int y, int z)
        {
            if (Smoothfactor == 0) return Noise3(x, y, z);

            var directSides = (Noise3(x + 1, y, z) + Noise3(x - 1, y, z) + Noise3(x, y + 1, z) + Noise3(x, y - 1, z) + Noise3(x, y, z - 1) + Noise3(x, y, z + 1)) / (6 * Smoothfactor);

            var indirectSides = (Noise3(x + 1, y + 1, z) + Noise3(x - 1, y - 1, z) + Noise3(x - 1, y + 1, z) + Noise3(x + 1, y - 1, z) +
                                 Noise3(x + 1, y, z - 1) + Noise3(x - 1, y, z - 1) + Noise3(x, y + 1, z - 1) + Noise3(x, y - 1, z - 1) +
                                 Noise3(x + 1, y, z + 1) + Noise3(x - 1, y, z + 1) + Noise3(x, y + 1, z + 1) + Noise3(x, y - 1, z + 1)) /
                                (12 * (float) Math.Sqrt(2) * Smoothfactor);

            var corners = (Noise3(x + 1, y + 1, z - 1) + Noise3(x - 1, y - 1, z - 1) + Noise3(x - 1, y + 1, z - 1) + Noise3(x + 1, y - 1, z - 1) +
                           Noise3(x + 1, y + 1, z + 1) + Noise3(x - 1, y - 1, z + 1) + Noise3(x - 1, y + 1, z + 1) + Noise3(x + 1, y - 1, z + 1)) /
                          (8 * (float) Math.Sqrt(3) * Smoothfactor);


            var center = Noise3(x, y, z) / (3 * Smoothfactor);

            return center + directSides + indirectSides + corners;
        }

        #endregion

        #region InterpolatedSNoise

        private float InterpolatedSNoise(float x)
        {
            var integer_X = (int) x;
            if (x < 0) integer_X--;
            var fractional_X = x - integer_X;

            var v1 = SmoothedNoise(integer_X);
            var v2 = SmoothedNoise(integer_X + 1);


            return LinearInterpolation(v1, v2, fractional_X);
        }

        private float InterpolatedSNoise2(float x, float y)
        {
            var integer_X = (int) x;
            if (x < 0) integer_X--;
            var fractional_X = x - integer_X;


            var integer_Y = (int) y;
            if (y < 0) integer_Y--;
            var fractional_Y = y - integer_Y;

            var v1 = SmoothedNoise2(integer_X, integer_Y);
            var v2 = SmoothedNoise2(integer_X + 1, integer_Y);
            var v3 = SmoothedNoise2(integer_X, integer_Y + 1);
            var v4 = SmoothedNoise2(integer_X + 1, integer_Y + 1);


            return LinearInterpolation2(v1, v2, v3, v4, fractional_X, fractional_Y);
        }

        private float InterpolatedSNoise3(float x, float y, float z)
        {
            var integer_X = (int) x;
            if (x < 0) integer_X--;
            var fractional_X = x - integer_X;

            var integer_Y = (int) y;
            if (y < 0) integer_Y--;
            var fractional_Y = y - integer_Y;

            var integer_Z = (int) z;
            if (z < 0) integer_Z--;
            var fractional_Z = z - integer_Z;

            var v1 = SmoothedNoise3(integer_X, integer_Y, integer_Z);
            var v2 = SmoothedNoise3(integer_X + 1, integer_Y, integer_Z);
            var v3 = SmoothedNoise3(integer_X, integer_Y + 1, integer_Z);
            var v4 = SmoothedNoise3(integer_X + 1, integer_Y + 1, integer_Z);

            var v5 = SmoothedNoise3(integer_X, integer_Y, integer_Z + 1);
            var v6 = SmoothedNoise3(integer_X + 1, integer_Y, integer_Z + 1);
            var v7 = SmoothedNoise3(integer_X, integer_Y + 1, integer_Z + 1);
            var v8 = SmoothedNoise3(integer_X + 1, integer_Y + 1, integer_Z + 1);

            return LinearInterpolation3(v1, v2, v3, v4, v5, v6, v7, v8, fractional_X, fractional_Y, fractional_Z);
        }

        #endregion

        #region PerlinAlgorithm

        public float[] PerlinNoise(int startX, int width)
        {
            var noiseLayers = new float[Octaves, width];

            if (Sizefactor < 1) Sizefactor = 1;


            for (var i = 0; i < Octaves; i++)
            {
                var frequency = (float) Math.Pow(2, i);
                var amplitude = (float) Math.Pow(Persistance, i);

                for (var x = 0; x < width; x++) noiseLayers[i, x] = InterpolatedSNoise((float) (x + startX) / Sizefactor * frequency) * amplitude;
            }

            var finishLayer = new float[width];

            for (var x = 0; x < width; x++)
            for (var i = 0; i < Octaves; i++)
                finishLayer[x] += noiseLayers[i, x];

            return finishLayer;
        }

        public float[,] PerlinNoise2(int startX, int startY, int width, int heigth)
        {
            var noiseLayers = new float[Octaves, width, heigth];

            if (Sizefactor < 1) Sizefactor = 1;


            for (var i = 0; i < Octaves; i++)
            {
                var frequency = (float) Math.Pow(2, i);
                var amplitude = (float) Math.Pow(Persistance, i);

                for (var x = 0; x < width; x++)
                for (var y = 0; y < heigth; y++)
                    noiseLayers[i, x, y] = InterpolatedSNoise2((float) (x + startX) / Sizefactor * frequency, (float) (y + startY) / Sizefactor * frequency) * amplitude;
            }

            var finishLayer = new float[width, heigth];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < heigth; y++)
            for (var i = 0; i < Octaves; i++)
                finishLayer[x, y] += noiseLayers[i, x, y];

            return finishLayer;
        }

        public float[,,] PerlinNoise3(int startX, int startY, int startZ, int width, int heigth, int depth)
        {
            var noiseLayers = new float[Octaves, width, heigth, depth];

            if (Sizefactor < 1) Sizefactor = 1;


            for (var i = 0; i < Octaves; i++)
            {
                var frequency = (float) Math.Pow(2, i);
                var amplitude = (float) Math.Pow(Persistance, i);

                for (var x = 0; x < width; x++)
                for (var y = 0; y < heigth; y++)
                for (var z = 0; z < depth; z++)
                    noiseLayers[i, x, y, z] = InterpolatedSNoise3((float) (x + startX) / Sizefactor * frequency, (float) (y + startY) / Sizefactor * frequency, (float) (z + startZ) / Sizefactor * frequency) * amplitude;
            }

            var finishLayer = new float[width, heigth, depth];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < heigth; y++)
            for (var z = 0; z < depth; z++)
            for (var i = 0; i < Octaves; i++)
                finishLayer[x, y, z] += noiseLayers[i, x, y, z];

            return finishLayer;
        }

        public float PerlinNoise3Web(float x, float y, float z)
        {
            float total = 0;

            for (var i = 0; i < Octaves; i++)
            {
                var frequency = (float) Math.Pow(2, i);
                var amplitude = (float) Math.Pow(Persistance, i);

                total += InterpolatedSNoise3(x * frequency, y * frequency, z * frequency) * amplitude;
            }

            return total;
        }


        //public float[,,] PerlinNoiseTest2(float persistance, int octaves, int sizefactor, int width, int heigth)
        //{

        //    float[, ,] noiseLayers = new float[octaves + 1, width, heigth];

        //    if (sizefactor < 1) sizefactor = 1;


        //    for (int i = 0; i < octaves; i++)
        //    {

        //        int frequency = (int)Math.Pow(2, i);
        //        float amplitude = (float)Math.Pow(persistance, octaves - i);

        //        for (int x = 0; x < width; x++)
        //        {
        //            for (int y = 0; y < heigth; y++)
        //            {

        //                noiseLayers[i, x, y] = InterpolatedSNoise2(((float)x) / frequency / sizefactor, ((float)y) / frequency / sizefactor) * amplitude;

        //            }
        //        }
        //    }

        //    //float[,] finishLayer = new float[width, heigth];

        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < heigth; y++)
        //        {
        //            for (int i = 0; i < octaves; i++)
        //            {


        //                noiseLayers[octaves, x, y] += noiseLayers[i, x, y];

        //            }
        //        }
        //    }

        //    return noiseLayers;
        //}

        #endregion

        #endregion
    }
}