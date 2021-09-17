namespace pocket.Logs.Core.Data
{
    public class PlayerRatingField
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int PlayerRatingId { get; set; }

        public string Key { get; set; } = string.Empty;

        public double Value { get; set; }
        
        public virtual Player Player { get; set; }

        public virtual PlayerRating Rating { get; set; }
    }
}