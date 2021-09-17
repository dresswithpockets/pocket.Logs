using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace pocket.Logs.Core.Rating.TrueSkill
{
    public class TrueSkill
    {
        public const double DefaultMu = 25.0;
        public const double DefaultSigma = DefaultMu / 3.0;
        public const double DefaultBeta = DefaultSigma / 2.0;
        public const double DefaultTau = DefaultSigma / 100.0;
        public const double DefaultDrawProbability = 0.10;
        public const double DefaultDelta = 0.0001;

        public static TrueSkill DefaultEnv { get; set; } = new();

        public double Mu { get; }
        public double Sigma { get; }
        public double Beta { get; }
        public double Tau { get; }
        public double DrawProbability { get; }

        public TrueSkill(double mu = DefaultMu, double sigma = DefaultSigma, double beta = DefaultBeta,
            double tau = DefaultTau, double drawProbability = DefaultDrawProbability)
        {
            Mu = mu;
            Sigma = sigma;
            Beta = beta;
            Tau = tau;
            DrawProbability = drawProbability;
        }

        public Rating CreateRating(double? mu, double? sigma) => new(mu ?? Mu, sigma ?? Sigma);

        public double MeanVariationWin(double diff, double drawMargin)
        {
            var x = diff - drawMargin;
            var denom = Mathematics.CumulativeDistribution(x);
            return denom == 0 ? -x : Mathematics.ProbabilityDensity(x) / denom;
        }

        public double MeanVariationDraw(double diff, double drawMargin)
        {
            var absDiff = Math.Abs(diff);
            var a = drawMargin - absDiff;
            var b = -drawMargin - absDiff;
            var denom = Mathematics.CumulativeDistribution(a) - Mathematics.CumulativeDistribution(b);
            var numer = Mathematics.ProbabilityDensity(a) - Mathematics.ProbabilityDensity(b);
            var result = denom == 0 ? a : numer / denom;
            var sign = diff < 0 ? -1 : 1;
            return result * sign;
        }

        public double StdDevVariationWin(double diff, double drawMargin)
        {
            var x = diff - drawMargin;
            var v = MeanVariationWin(diff, drawMargin);
            var w = v * (v + x);
            if (w is > 0 and < 1) return w;
            throw new Exception("Cannot calculate correctly");
        }

        public double StdDevVariationDraw(double diff, double drawMargin)
        {
            var absDiff = Math.Abs(diff);
            var a = drawMargin - absDiff;
            var b = -drawMargin - absDiff;
            var denom = Mathematics.CumulativeDistribution(a) - Mathematics.CumulativeDistribution(b);
            if (denom == 0) throw new Exception("Cannot calculate correctly");
            var v = MeanVariationDraw(absDiff, drawMargin);
            return Math.Pow(v, 2.0) +
                   (a * Mathematics.ProbabilityDensity(a) - b * Mathematics.ProbabilityDensity(b)) / denom;
        }
        
        // TODO: what type should Keys be?
        public (Rating[][] ratingGroups, int[] Keys) ValidateRatingGroups(Rating[][] ratingGroups) =>
            throw new NotImplementedException();

        public (Rating[][] ratingGroups, TKey[] Keys)
            ValidateRatingGroups<TKey>(Dictionary<TKey, Rating>[] ratingGroups) where TKey : notnull =>
            throw new NotImplementedException(); 

        public double[][] ValidateWeights(double[][]? weights, Rating[][] ratingGroups) =>
            weights ?? ratingGroups.Select(g => Enumerable.Repeat(1.0, g.Length).ToArray()).ToArray();
        
        // TODO: dictionary validate weights
        
        // TODO: factor_graph_builders, run_schedule?

        public Dictionary<TKey, Rating>[] Rate<TKey>(Dictionary<TKey, Rating>[] ratingGroups,
            Dictionary<TKey, double>[] weights, double minDelta = DefaultDelta) where TKey : notnull
        {
            /*var (groups, keys) = ValidateRatingGroups(ratingGroups);
            var rated = Rate(groups, weights, minDelta);*/
            throw new NotImplementedException();
        }

        public Rating[][] Rate(Rating[][] ratingGroups, double[][] weights, double minDelta = DefaultDelta)
        {
            throw new NotImplementedException();
        }
    }
}