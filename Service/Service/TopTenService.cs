using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using Microsoft.EntityFrameworkCore;
using Service.Interface;

namespace Service.Service
{
    public class TopTenService : ITopTenService
    {
        private readonly ApplicationDbContext _context;

        public TopTenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TopPlayerDto> GetTopBySets()
        {
            return GetAllPlayersStats()
                .OrderByDescending(p => p.TotalSetsWon)
                .Take(10)
                .ToList();
        }

        public List<TopPlayerDto> GetTopByWinRatio()
        {
            return GetAllPlayersStats()
                .Where(p => p.MatchesPlayed > 0)
                .OrderByDescending(p => p.WinRatio)
                .ThenByDescending(p => p.MatchesPlayed)
                .Take(10)
                .ToList();
        }

        private IEnumerable<TopPlayerDto> GetAllPlayersStats()
        {
            var players = _context.Players
                .Include(p => p.MatchesAsPlayer1)
                .Include(p => p.MatchesAsPlayer2)
                .ToList();

            return players.Select(player =>
            {
                var allMatches = player.MatchesAsPlayer1
                                    .Concat(player.MatchesAsPlayer2)
                                    .Where(m => m.IsFinished)
                                    .ToList();

                int matchesPlayed = allMatches.Count;

                int matchesWon = allMatches.Count(m =>
                    (m.Player1Id == player.Id && m.Player1Score > m.Player2Score) ||
                    (m.Player2Id == player.Id && m.Player2Score > m.Player1Score));

                int setsWon = allMatches.Sum(m =>
                    m.Player1Id == player.Id ? m.Player1Score :
                    m.Player2Id == player.Id ? m.Player2Score : 0);

                int winRatioPercent = matchesPlayed > 0
                    ? (int)Math.Round((double)matchesWon / matchesPlayed * 100)
                    : 0;

                return new TopPlayerDto
                {
                    Id = player.Id,
                    Name = player.Name,
                    MatchesPlayed = matchesPlayed,
                    TotalSetsWon = setsWon,
                    MatchesWon = matchesWon,
                    WinRatio = winRatioPercent
                };
            });
        }

        public List<TopMatchDto> GetLongestMatches(int count = 10)
        {
            return _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Sets)
                .Where(m => m.IsFinished)
                .ToList()
                .Select(m => new TopMatchDto
                {
                    MatchId = m.Id,
                    Player1Name = m.Player1.Name,
                    Player2Name = m.Player2.Name,
                    MatchDate = m.MatchDate,
                    TotalDuration = TimeSpan.FromTicks(
                        m.Sets
                         .Where(s => s.SetDuration.HasValue)
                         .Sum(s => s.SetDuration.Value.Ticks)
                    )
                })
                .OrderByDescending(x => x.TotalDuration)
                .Take(count)
                .ToList();
        }


        public List<TopMatchDto> GetShortestMatches(int count = 10)
        {
            return _context.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Sets)
                .Where(m => m.IsFinished)
                .ToList()
                .Select(m => new
                {
                    Match = m,
                    TotalDuration = m.Sets
                        .Where(s => s.SetDuration.HasValue)
                        .Sum(s => s.SetDuration.Value.Ticks)
                })
                .OrderBy(x => x.TotalDuration)
                .Take(count)
                .Select(x => new TopMatchDto
                {
                    MatchId = x.Match.Id,
                    Player1Name = x.Match.Player1.Name,
                    Player2Name = x.Match.Player2.Name,
                    MatchDate = x.Match.MatchDate,
                    TotalDuration = TimeSpan.FromTicks(x.TotalDuration)
                })
                .ToList();
        }

    }
}
