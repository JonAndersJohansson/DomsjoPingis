using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using DataAccessLayer.DTO_s;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Service.Interface;

namespace Service.Service
{
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _dbContext;

        public MatchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SelectListItem> GetSetList()
        {
            return Enum.GetValues<BestOf>()
                .Select(b => new SelectListItem
                {
                    Value = ((int)b).ToString(),
                        Text = $"{(int)b} set"
                }).ToList();
        }

        public int CreateGame(int player1Id, int player2Id, BestOf bestOf)
        {
            var match = new Match
            {
                Player1Id = player1Id,
                Player2Id = player2Id,
                BestOfSets = bestOf,
                MatchDate = DateTime.Now,
                IsFinished = false,
                Sets = new List<Set>()
            };

            _dbContext.Matches.Add(match);
            _dbContext.SaveChanges();
            StartNewSet(match.Id);

            return match.Id;
        }

        public MatchDto GetById(int matchId)
        {
            var matchDto = _dbContext.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Sets)
                .Where(m => m.Id == matchId)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Player1Id = m.Player1Id,
                    Player2Id = m.Player2Id,
                    Player1Name = m.Player1.Name,
                    Player2Name = m.Player2.Name,
                    BestOfSets = m.BestOfSets,
                    MatchDate = m.MatchDate,
                    Player1Score = m.Player1Score,
                    Player2Score = m.Player2Score,
                    Player1Gender = m.Player1.Gender,
                    Player2Gender = m.Player2.Gender,
                    IsP1FirstServer = m.IsP1FirstServer,
                    IsFinished = m.IsFinished,
                    IsActive = m.IsActive,
                    Sets = m.Sets
                        .OrderBy(s => s.SetNumber)
                        .Select(s => new SetDto
                        {
                            Id = s.Id,
                            MatchId = s.MatchId,
                            SetNumber = s.SetNumber,
                            Player1Points = s.Player1Points,
                            Player2Points = s.Player2Points,
                            SetStartTime = s.SetStartTime,//SET
                            SetDuration = s.SetDuration,//SET
                            IsFinished = s.IsFinished
                        }).ToList()
                })
                .FirstOrDefault();

            return matchDto;
        }

        public void HandlePoint(int matchId, int player, bool isToAdd)
        {
            var match = _dbContext.Matches
                .Include(m => m.Sets)
                .FirstOrDefault(m => m.Id == matchId);

            if (match == null)
                throw new Exception("Match not found");

            if (match.Sets == null)
                match.Sets = new List<Set>();

            var currentSet = match.Sets.LastOrDefault(s => !s.IsFinished);
            if (currentSet == null)
            {
                currentSet = new Set { MatchId = matchId, SetNumber = match.Sets.Count + 1 };
                _dbContext.Sets.Add(currentSet);
            }

            if (isToAdd)
            {
                if (player == 1) currentSet.Player1Points++;
                if (player == 2) currentSet.Player2Points++;
            }
            else
            {
                if (player == 1 && currentSet.Player1Points > 0)
                    currentSet.Player1Points--;

                if (player == 2 && currentSet.Player2Points > 0)
                    currentSet.Player2Points--;
            }

            if (CheckEndOfSet(currentSet))
            {
                currentSet.IsFinished = true;

                currentSet.SetDuration = DateTime.UtcNow - currentSet.SetStartTime; //SET

                if (currentSet.Player1Points > currentSet.Player2Points)
                    match.Player1Score++;
                else
                    match.Player2Score++;

                if (IsMatchWon(match))
                {
                    match.IsFinished = true;
                }
                else
                {
                    StartNewSet(match.Id);
                }
            }

            _dbContext.SaveChanges();
        }

        public SetDto? GetCurrentSet(int matchId)
        {
            var set = _dbContext.Sets
                .Where(s => s.MatchId == matchId && !s.IsFinished)
                .OrderByDescending(s => s.SetNumber)
                .FirstOrDefault();

            return set == null ? null : new SetDto
            {
                Id = set!.Id,
                MatchId = set.MatchId,
                SetNumber = set.SetNumber,
                Player1Points = set.Player1Points,
                Player2Points = set.Player2Points,
                IsFinished = set.IsFinished,
                SetDuration = set.SetDuration,
                SetStartTime = set.SetStartTime,
            };
        }

        public SetDto UpdateSetStatusFlags(SetDto setDto, MatchDto matchDto)
        {
            int p1 = setDto.Player1Points;
            int p2 = setDto.Player2Points;
            int totalPoints = p1 + p2;
            bool isOvertime = totalPoints >= 20;

            bool isOddSet = setDto.SetNumber % 2 != 0;
            bool p1StartsSet;
            if (matchDto.IsP1FirstServer.HasValue)
            {
                if (matchDto.IsP1FirstServer.Value)
                    p1StartsSet = isOddSet;
                else
                    p1StartsSet = !isOddSet;
            }
            else
            {
                p1StartsSet = true;
            }

            if (isOvertime)
            {
                setDto.IsP1Serving = (totalPoints % 2 == 0) ? p1StartsSet : !p1StartsSet;
            }
            else
            {
                int serveIndex = totalPoints / 2;
                setDto.IsP1Serving = (serveIndex % 2 == 0) ? p1StartsSet : !p1StartsSet;
            }

            setDto.IsDeuce = p1 >= 10 && p2 >= 10 && p1 == p2;

            int maxPoints = Math.Max(p1, p2);
            int diff = Math.Abs(p1 - p2);
            bool canWinSet = maxPoints >= 10 && diff >= 1;

            setDto.IsSetBallPlayer1 = canWinSet && p1 > p2;
            setDto.IsSetBallPlayer2 = canWinSet && p2 > p1;

            int setsToWin = (int)matchDto.BestOfSets / 2 + 1;
            bool p1IsOneSetFromVictory = matchDto.Player1Score == setsToWin - 1;
            bool p2IsOneSetFromVictory = matchDto.Player2Score == setsToWin - 1;

            setDto.IsMatchBallPlayer1 = setDto.IsSetBallPlayer1 && p1IsOneSetFromVictory;
            setDto.IsMatchBallPlayer2 = setDto.IsSetBallPlayer2 && p2IsOneSetFromVictory;

            if (setDto.IsMatchBallPlayer1)
                setDto.IsSetBallPlayer1 = false;
            if (setDto.IsMatchBallPlayer2)
                setDto.IsSetBallPlayer2 = false;

            return setDto;
        }

        public void StartNewSet(int matchId)
        {
            var setCount = _dbContext.Sets.Count(s => s.MatchId == matchId);
            _dbContext.Sets.Add(new Set
            {
                MatchId = matchId,
                SetNumber = setCount + 1,
                SetStartTime = DateTime.UtcNow
            });
            _dbContext.SaveChanges();
        }

        private bool CheckEndOfSet(Set set)
        {
            return (set.Player1Points >= 11 || set.Player2Points >= 11) &&
                   Math.Abs(set.Player1Points - set.Player2Points) >= 2;
        }

        private bool IsMatchWon(Match match)
        {
            int neededWins = (int)match.BestOfSets / 2 + 1;
            return match.Player1Score >= neededWins || match.Player2Score >= neededWins;
        }

        public List<SelectListItem> GetPlayerList()
        {
            return _dbContext.Players
                             .OrderBy(p => p.Name)
                             .Select(p => new SelectListItem
                             {
                                 Value = p.Id.ToString(),
                                 Text = p.Name
                             })
                             .ToList();
        }

        public void UpdateMatch(MatchDto updatedMatch)
        {
            var match = _dbContext.Matches
                .Include(m => m.Sets)
                .FirstOrDefault(m => m.Id == updatedMatch.Id);

            if (match == null)
                throw new KeyNotFoundException($"Match med ID {updatedMatch.Id} hittades inte.");
            match.Player1Id = updatedMatch.Player1Id;
            match.Player2Id = updatedMatch.Player2Id;
            match.BestOfSets = updatedMatch.BestOfSets;
            match.MatchDate = updatedMatch.MatchDate;
            match.Player1Score = updatedMatch.Player1Score;
            match.Player2Score = updatedMatch.Player2Score;
            match.IsP1FirstServer = updatedMatch.IsP1FirstServer;
            match.IsFinished = updatedMatch.IsFinished;
            foreach (var setDto in updatedMatch.Sets ?? Enumerable.Empty<SetDto>())
            {
                var setEntity = match.Sets.FirstOrDefault(s => s.Id == setDto.Id);
                if (setEntity != null)
                {
                    setEntity.Player1Points = setDto.Player1Points;
                    setEntity.Player2Points = setDto.Player2Points;
                    setEntity.IsFinished = setDto.IsFinished;
                }
                else if (setDto.SetNumber <= (int)updatedMatch.BestOfSets)
                {
                    var newSet = new Set
                    {
                        MatchId = match.Id,
                        SetNumber = setDto.SetNumber,
                        Player1Points = setDto.Player1Points,   
                        Player2Points = setDto.Player2Points,   
                        IsFinished = setDto.IsFinished,
                        SetStartTime = DateTime.UtcNow
                    };
                    _dbContext.Sets.Add(newSet);
                }
            }


            _dbContext.SaveChanges();
        }

    }
}
