using pocket.Logs.Core.Skills.Numerics;

namespace pocket.Logs.Core.Skills
{
    public class TeamsRange : Range<TeamsRange>
    {
        public TeamsRange()
            : base(int.MinValue, int.MinValue)
        {
        }

        private TeamsRange(int min, int max)
            : base(min, max)
        {
        }

        protected override TeamsRange Create(int min, int max)
        {
            return new TeamsRange(min, max);
        }
    }
}