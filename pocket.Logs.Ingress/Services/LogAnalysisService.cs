using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pocket.Logs.Core.Data;
using pocket.Logs.Core.Skills;
using Player = pocket.Logs.Core.Data.Player;
using Rating = pocket.Logs.Core.Skills.Rating;

namespace pocket.Logs.Ingress.Services
{
    public class LogAnalysisService
    {
        private readonly LogsContext _context;
        private readonly ILogClient _logClient;
        private readonly ILogger<LogAnalysisService> _logger;

        public LogAnalysisService(LogsContext context, ILogClient logClient, ILogger<LogAnalysisService> logger)
        {
            _context = context;
            _logClient = logClient;
            _logger = logger;
        }

        public async Task AnalyzeAsync(RetrievedLog retrievedLog, CancellationToken cancellationToken = default)
        {
            var log = await _logClient.GetLog(retrievedLog.LogId);
            var redLog = log.Teams[TeamName.Red];
            var blueLog = log.Teams[TeamName.Blue];

            var steamIds = log.Players.Select(p => p.Key).ToArray();
            var players = await _context.Players.Where(p => steamIds.Contains(p.SteamId))
                .ToListAsync(cancellationToken);

            // add missing players to the database
            var logPlayersNotFound = log.Players.Where(s => players.All(p => p.SteamId != s.Key))
                .Select(p => new Player { SteamId = p.Key }).ToList();

            foreach (var player in logPlayersNotFound)
            {
                var rating = new PlayerRating
                {
                    Model = PlayerRatingModel.TrueSkill,
                    Player = player,
                };

                var ratingFields = new List<PlayerRatingField>
                {
                    new()
                    {
                        Player = player, Rating = rating, Key = "mean", Value = GameInfo.DefaultGameInfo.InitialMean
                    },
                    new()
                    {
                        Player = player, Rating = rating, Key = "standardDeviation",
                        Value = GameInfo.DefaultGameInfo.InitialStandardDeviation
                    }
                };
                await _context.AddAsync(player, cancellationToken);
                await _context.AddAsync(rating, cancellationToken);
                await _context.AddRangeAsync(ratingFields, cancellationToken);
            }

            var playerPairs = players.Join(log.Players, p => p.SteamId, p => p.Key, (p, kvp) => (Player: p, Log: kvp.Value)).ToList();

            var teamPlayerPairs = playerPairs.GroupBy(p => p.Log.Team).ToList();

            var redPlayersGroup = teamPlayerPairs.FirstOrDefault(g => g.Key == TeamName.Red);
            var bluePlayersGroup = teamPlayerPairs.FirstOrDefault(g => g.Key == TeamName.Blue);

            if (redPlayersGroup == null || bluePlayersGroup == null)
                return; // skip invalid log
            
            var redRatings = redPlayersGroup.ToDictionary(p => p.Player.Id, p =>
            {
                var rating = p.Player.Ratings.First(r => r.Model == PlayerRatingModel.TrueSkill);
                var mean = rating.Fields.First(f => f.Key == "mean").Value;
                var stdDev = rating.Fields.First(f => f.Key == "standardDeviation").Value;
                return new Rating(mean, stdDev);
            });

            var blueRatings = bluePlayersGroup.ToDictionary(p => p.Player.Id, p =>
            {
                var rating = p.Player.Ratings.First(r => r.Model == PlayerRatingModel.TrueSkill);
                var mean = rating.Fields.First(f => f.Key == "mean").Value;
                var stdDev = rating.Fields.First(f => f.Key == "standardDeviation").Value;
                return new Rating(mean, stdDev);
            });
            
            // do rating analysis
            var teams = new[] { redRatings, blueRatings };
            var gameInfo = GameInfo.DefaultGameInfo;
            
            _ = TrueSkillCalculator.CalculateMatchQuality(gameInfo, teams); // TODO: use match quality?
            var result = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, redLog.Score >= blueLog.Score ? 1 : 2,
                redLog.Score > blueLog.Score ? 2 : 1);
            
            // update new ratings in database for each player
            foreach (var (playerId, rating) in result)
            {
                var ratingFields = await _context.PlayerRatingFields
                    .Where(f => f.PlayerId == playerId && f.Rating.Model == PlayerRatingModel.TrueSkill)
                    .ToDictionaryAsync(f => f.Key, cancellationToken);
                ratingFields["mean"].Value = rating.Mean;
                ratingFields["standardDeviation"].Value = rating.StandardDeviation;
            }

            retrievedLog.Processed = true;
            _context.Update(retrievedLog);

            await _context.SaveChangesAsync(cancellationToken);

            // TODO: add match to db and relate to db players
            // TODO: aggregate data, save stats
        }
    }
}