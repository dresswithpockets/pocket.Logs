using Dapper.SimpleRepository;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Interfaces;

namespace pocket.Logs.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        public Repository<RetrievedLog> RetrievedLogs { get; }

        public Repository<Player> Players { get; }

        public Repository<PlayerRating> PlayerRatings { get; }

        public Repository<PlayerRatingField> PlayerRatingFields { get; }

        public UnitOfWork(ISqlConnectionProvider sqlConnectionProvider)
        {
            var defaultConnectionString = sqlConnectionProvider.GetConnectionString();
            RetrievedLogs = new Repository<RetrievedLog>(defaultConnectionString);
            Players = new Repository<Player>(defaultConnectionString);
            PlayerRatings = new Repository<PlayerRating>(defaultConnectionString);
            PlayerRatingFields = new Repository<PlayerRatingField>(defaultConnectionString);
        }
    }
}