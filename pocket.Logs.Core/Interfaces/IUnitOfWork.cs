using Dapper.SimpleRepository;
using pocket.Logs.Core.Data;

namespace pocket.Logs.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Repository<RetrievedLog> RetrievedLogs { get; }
        Repository<Player> Players { get; }
        Repository<PlayerRating> PlayerRatings { get; }
        Repository<PlayerRatingField> PlayerRatingFields { get; }
    }
}