using System;

namespace pocket.Logs.Core.Rating.TrueSkill
{
    public static class Mathematics
    {
        public static double ComplementaryError(double x)
        {
            var z = Math.Abs(x);
            var t = 1.0 / (1.0 + z / 2.0);
            var r = t * Math.Exp(-z * z - 1.26551223 + t * (1.00002368 + t * (
                0.37409196 + t * (0.09678418 + t * (-0.18628806 + t * (
                    0.27886807 + t * (-1.13520398 + t * (1.48851587 + t * (
                        -0.82215223 + t * 0.17087277
                    )))
                )))
            )));
            return x < 0.0 ? 2.0 - r : r;
        }

        public static double InverseComplementaryError(double y)
        {
            if (y >= 2.0) return -100.0;
            if (y <= 0.0) return 100.0;
            var zeroPoint = y < 1.0;
            if (!zeroPoint) y = 2.0 - y;
            var t = Math.Sqrt(-2.0 * Math.Log(y / 2.0));
            var x = -0.70711 * ((2.30753 + t * 0.27061) / (1.0 + t * (0.99229 + t * 0.04481)) - t);
            for (var i = 0; i < 2; i++)
            {
                var err = ComplementaryError(x) - y;
                x += err / (1.12837916709551257 * Math.Exp(-Math.Pow(x, 2)) - x * err);
            }

            return zeroPoint ? x : -x;
        }

        public static double CumulativeDistribution(double x, double mu = 0.0, double sigma = 1.0) =>
            0.5 * ComplementaryError(-(x - mu) / (sigma * Math.Sqrt(2)));

        public static double ProbabilityDensity(double x, double mu = 0.0, double sigma = 1.0) =>
            1 / Math.Sqrt(2 * Math.PI) * Math.Abs(sigma) * Math.Exp(-(Math.Pow((x - mu) / Math.Abs(sigma), 2) / 2));

        public static double InverseCumulativeDistribution(double x, double mu = 0.0, double sigma = 1.0) =>
            mu - sigma * Math.Sqrt(2) * InverseComplementaryError(2 * x);
    }
}