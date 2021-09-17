namespace pocket.Logs.Core.Rating.TrueSkill
{
    public class Rating : Gaussian
    {
        public Rating(Gaussian gaussian) : base(0.0, 0.0, gaussian.Mu, gaussian.Sigma)
        {
        }

        public Rating(double mu, double sigma) : base(0.0, 0.0, mu, sigma)
        {
        }
    }
}