using pocket.Logs.Core.Skills.Numerics;

namespace pocket.Logs.Core.Skills
{
    public class PlayersRange : Range<PlayersRange>
    {
        public PlayersRange()
            : base(int.MinValue, int.MinValue)
        {
        }

        private PlayersRange(int min, int max)
            : base(min, max)
        {
        }

        protected override PlayersRange Create(int min, int max)
        {
            return new PlayersRange(min, max);
        }
    }
}