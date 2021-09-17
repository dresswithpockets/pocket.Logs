using System.Collections.Generic;

namespace pocket.Logs.Core.Data
{
    public class Player
    {
        public int Id { get; set; }

        public string SteamId { get; set; } = string.Empty;

        public virtual ICollection<PlayerRating> Ratings { get; set; }
    }
}