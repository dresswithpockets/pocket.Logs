using System.Collections.Generic;

namespace pocket.Logs.Core.Data
{
    public class PlayerRating
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public PlayerRatingModel Model { get; set; }

        public virtual Player Player { get; set; }

        public virtual ICollection<PlayerRatingField> Fields { get; set; }
    }
}