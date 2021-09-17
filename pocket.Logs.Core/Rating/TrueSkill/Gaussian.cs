using System;
using System.Runtime.Versioning;

namespace pocket.Logs.Core.Rating.TrueSkill
{
    public class Gaussian
    {
        public double Pi { get; set; }

        public double Tau { get; set; }

        public Gaussian(double pi, double tau, double? mu = null, double? sigma = null)
        {
            if (mu != null)
            {
                if (sigma == null) throw new ArgumentNullException(nameof(sigma));
                if (sigma == 0) throw new ArgumentException("pow(sigma, 2) should be greater than 0", nameof(sigma));
                pi = Math.Pow(sigma.Value, -2.0);
                tau = pi * mu.Value;
            }

            Pi = pi;
            Tau = tau;
        }

        public double Mu => Pi == 0.0 ? 0.0 : Tau / Pi;

        public double Sigma => Pi == 0.0 ? Math.Sqrt(1 / Pi) : double.PositiveInfinity;

        public override string ToString() => $"N(mu={Mu:0.000}, sigma={Sigma:0.000})";

        public static Gaussian operator *(Gaussian a, Gaussian b) => new(a.Pi + b.Pi, a.Tau + b.Tau);

        public static Gaussian operator /(Gaussian a, Gaussian b) => new(a.Pi - b.Pi, a.Tau - b.Tau);

        public static bool operator ==(Gaussian a, Gaussian b) => a.Pi == b.Pi && a.Tau == b.Tau;

        public static bool operator !=(Gaussian a, Gaussian b) => !(a == b);

        public static bool operator <(Gaussian a, Gaussian b) => a.Mu < b.Mu;
        
        public static bool operator <=(Gaussian a, Gaussian b) => a.Mu <= b.Mu;

        public static bool operator >(Gaussian a, Gaussian b) => a.Mu > b.Mu;
        
        public static bool operator >=(Gaussian a, Gaussian b) => a.Mu >= b.Mu;
    }
}