using System;

namespace Noise{
    public static class CubicNoise
    {
        private const int CUBIC_NOISE_RAND_A = 134775813;
        private const int CUBIC_NOISE_RAND_B = 1103515245;

        private static float CubicNoiseRandom(uint seed, int x, int y)
        {
            return (float) ((((x ^ y) * CUBIC_NOISE_RAND_A) ^ (seed + x)) *
                            (((CUBIC_NOISE_RAND_B * x) << 16) ^ (CUBIC_NOISE_RAND_B * y) - CUBIC_NOISE_RAND_A)) /
                   uint.MaxValue;
        }

        private static int CubicNoiseTile(int coordinate, int period)
        {
            return coordinate % period;
        }

        static float CubicNoiseInterpolate(float a, float b, float c, float d, float x)
        {
            float p = (d - c) - (a - b);
            return x * x * x * p + x * x * ((a - b) - p) + x * (c - a) + b;
        }

        public static CubicNoiseConfig CubicNoiseConfig1D(uint seed, int octave, int period)
        {
            CubicNoiseConfig config = new CubicNoiseConfig();

            config.seed = seed;
            config.octave = octave;
            config.periodx = period / octave;

            return config;
        }

        public static CubicNoiseConfig CubicNoiseConfig2D(uint seed, int octave, int periodx, int periody)
        {
            CubicNoiseConfig config;

            config.seed = seed;
            config.octave = octave;
            config.periodx = periodx / octave;
            config.periody = periody / octave;

            return config;
        }

        public static float CubicNoiseSample1D(CubicNoiseConfig config, float x)
        {
            int xi = (int) (x / config.octave);
            float lerp = x / config.octave - xi;

            return CubicNoiseInterpolate(
                       CubicNoiseRandom(config.seed, CubicNoiseTile(xi - 1, config.periodx), 0),
                       CubicNoiseRandom(config.seed, CubicNoiseTile(xi, config.periodx), 0),
                       CubicNoiseRandom(config.seed, CubicNoiseTile(xi + 1, config.periodx), 0),
                       CubicNoiseRandom(config.seed, CubicNoiseTile(xi + 2, config.periodx), 0),
                       lerp) * 0.5f + 0.25f;
        }

        public static float CubicNoiseSample2D( CubicNoiseConfig config, float x, float y)
        {
            int xi = (int) Math.Floor(x / config.octave);
            float lerpx = x / config.octave - xi;
            int yi = (int) Math.Floor(y / config.octave);
            float lerpy = y / config.octave - yi;

            float[] xSamples = new float[4];

            for (int i = 0; i < 4; ++i)
                xSamples[i] = CubicNoiseInterpolate(
                    CubicNoiseRandom(config.seed,
                        CubicNoiseTile(xi - 1, config.periodx),
                        CubicNoiseTile(yi - 1 + i, config.periody)),
                    CubicNoiseRandom(config.seed,
                        CubicNoiseTile(xi, config.periodx),
                        CubicNoiseTile(yi - 1 + i, config.periody)),
                    CubicNoiseRandom(config.seed,
                        CubicNoiseTile(xi + 1, config.periodx),
                        CubicNoiseTile(yi - 1 + i, config.periody)),
                    CubicNoiseRandom(config.seed,
                        CubicNoiseTile(xi + 2, config.periodx),
                        CubicNoiseTile(yi - 1 + i, config.periody)),
                    lerpx);

            return CubicNoiseInterpolate(xSamples[0], xSamples[1], xSamples[2], xSamples[3], lerpy) * 0.5f + 0.25f;
        }
    }

    public struct CubicNoiseConfig {
        public uint seed;
        public int octave;
        public int periodx, periody;
    }
}