using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.DTO_s;
using Microsoft.EntityFrameworkCore;
using Service.Interface;

namespace Service.Service
{
    public class PlayerStatisticsService : IPlayerStatisticsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public PlayerStatisticsService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Validation ConfirmInput(int player1)
        {
            var p1 = _dbContext.Players.Find(player1);

            if (p1 == null)
            {
                return Validation.P1NotFound;
            }
            else
            {
                return Validation.OK;
            }
        }

        public PlayerStatisticsDto GetHeadToHeadStats(int player1Id, int player2Id)
        {
            if (player1Id == player2Id)
                return null;

            var matches = _dbContext.Matches
                .Where(m => m.IsFinished &&
                    ((m.Player1Id == player1Id && m.Player2Id == player2Id) ||
                     (m.Player1Id == player2Id && m.Player2Id == player1Id)))
                .ToList();

            if (!matches.Any())
                return null;

            var player1Wins = matches.Count(m =>
                (m.Player1Id == player1Id && m.Player1Score > m.Player2Score) ||
                (m.Player2Id == player1Id && m.Player2Score > m.Player1Score));

            var player2Wins = matches.Count(m =>
                (m.Player1Id == player2Id && m.Player1Score > m.Player2Score) ||
                (m.Player2Id == player2Id && m.Player2Score > m.Player1Score));

            var player1Name = _dbContext.Players.FirstOrDefault(p => p.Id == player1Id)?.Name ?? "Spelare 1";
            var player2Name = _dbContext.Players.FirstOrDefault(p => p.Id == player2Id)?.Name ?? "Spelare 2";

            return new PlayerStatisticsDto
            {
                PlayerAName = player1Name,
                PlayerBName = player2Name,
                TotalMatches = matches.Count,
                PlayerAWins = player1Wins,
                PlayerBWins = player2Wins
            };
        }


        public PlayerStatisticsDto GetStatisticsForPlayer(int player1Id)
        {
            var player = _dbContext.Players.FirstOrDefault(p => p.Id == player1Id && p.IsActive);
            if (player == null)
                return null;

            var matches = _dbContext.Matches
                .Where(m => (m.Player1Id == player1Id || m.Player2Id == player1Id) && m.IsFinished)
                .Select(m => new
                {
                    Match = m,
                    Sets = m.Sets.Where(s => s.SetDuration.HasValue).ToList()
                })
                .ToList();

            int totalMatches = matches.Count;
            int wins = matches.Count(m =>
                (m.Match.Player1Id == player1Id && m.Match.Player1Score > m.Match.Player2Score)
                ||
                (m.Match.Player2Id == player1Id && m.Match.Player2Score > m.Match.Player1Score)
            );

            int losses = matches.Count(m =>
                (m.Match.Player1Id == player1Id && m.Match.Player1Score < m.Match.Player2Score)
                ||
                (m.Match.Player2Id == player1Id && m.Match.Player2Score < m.Match.Player1Score)
            );

            int winRatio = totalMatches > 0 ? (int)Math.Round((wins / (double)totalMatches) * 100) : 0;

            var durations = matches
                .Where(m => m.Sets.Any())
                .Select(m =>
                {
                    var start = m.Sets.Min(s => s.SetStartTime);
                    var end = m.Sets.Max(s => s.SetStartTime + s.SetDuration.Value);
                    return end - start;
                })
                .ToList();

            var averageDuration = durations.Any()
            ? TimeSpan.FromTicks((long)durations.Average(ts => ts.Ticks))
            : TimeSpan.Zero;


            var longestDuration = durations.Any() ? durations.Max() : TimeSpan.Zero;
            var shortestDuration = durations.Any() ? durations.Min() : TimeSpan.Zero;

            return new PlayerStatisticsDto
            {
                SelectedPlayerName = player.Name,
                TotalMatches = totalMatches,
                TotalWins = wins,
                TotalLosses = losses,
                WinRation = winRatio,
                AverageMatchDuration = averageDuration,
                LongestMatchDuration = longestDuration,
                ShortestMatchDuration = shortestDuration,
                AllPlayers = _dbContext.Players.Where(p => p.IsActive).ToList()
            };
        }

        public PlayerStatisticsDto PlayerBestAndWorst(int player1Id)
        {
            var matches = _dbContext.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Where(m => m.IsFinished && (m.Player1Id == player1Id || m.Player2Id == player1Id))
                .ToList();

            var opponentStats = matches
                .Select(m => new
                {
                    OpponentId = m.Player1Id == player1Id ? m.Player2Id : m.Player1Id,
                    OpponentName = m.Player1Id == player1Id
                        ? m.Player2?.Name ?? $"Id {m.Player2Id}"
                        : m.Player1?.Name ?? $"Id {m.Player1Id}",
                    IsWin = (m.Player1Id == player1Id && m.Player1Score > m.Player2Score) ||
                            (m.Player2Id == player1Id && m.Player2Score > m.Player1Score)
                })
                .Where(x => x.OpponentId != player1Id && !string.IsNullOrEmpty(x.OpponentName))
                .GroupBy(x => new { x.OpponentId, x.OpponentName })
                .Select(g => new
                {
                    OpponentName = g.Key.OpponentName,
                    Total = g.Count(),
                    Wins = g.Count(x => x.IsWin),
                    Losses = g.Count(x => !x.IsWin),
                    WinRate = g.Count(x => x.IsWin) / (double)g.Count()
                })
                .Where(x => x.Total > 0)
                .ToList();

            if (opponentStats.Count == 0)
            {
                return new PlayerStatisticsDto
                {
                    BestPerformance = "Ingen statistik mot andra spelare",
                    WorstPerformance = "Ingen statistik mot andra spelare"
                };
            }

            var best = opponentStats
                .Where(x => x.Wins > 0)
                .OrderByDescending(x => x.WinRate)
                .ThenByDescending(x => x.Total)
                .FirstOrDefault();

            var worst = opponentStats
                .Where(x => x.Losses > 0)
                .OrderBy(x => x.WinRate)
                .ThenByDescending(x => x.Total)
                .FirstOrDefault();

            return new PlayerStatisticsDto
            {
                BestPerformance = best != null
                ? $"{best.OpponentName}\n({best.Wins}/{best.Total} vinster, {best.WinRate:P0})"
:                "Ingen bästa motståndare",
                    WorstPerformance = worst != null
                ? $"{worst.OpponentName}\n({worst.Wins}/{worst.Total} vinster, {worst.WinRate:P0})"
                : "Ingen sämsta motståndare"
                };
        }
    }
}


