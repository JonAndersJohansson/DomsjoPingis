using DataAccessLayer.Data;
using DataAccessLayer.DTO_s;
using Service.Infrastructure.Paging;
using Service.Interface;

namespace Service.Service
{
    public class MatchHistoryService : IMatchHistoryService
    {
        private readonly ApplicationDbContext _dbContext;

        public MatchHistoryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MatchDto> GetMatches(int id)
        {
            var query = _dbContext.Matches
                .Where(c => c.Id == id && c.IsActive == true)
                .Select(c => new MatchDto
                {
                    Id = c.Id,
                    Player1Id = c.Player1Id,
                    Player1Name = c.Player1.Name,
                    Player2Id = c.Player2Id,
                    Player2Name = c.Player2.Name,
                    BestOfSets = c.BestOfSets,
                    MatchDate = c.MatchDate,
                    Player1Score = c.Player1Score,
                    Player2Score = c.Player2Score,
                    IsFinished = c.IsFinished
                });

            return query.ToList();
        }
        public MatchDto GetMatchById(int id)
        {
            var match = _dbContext.Matches
                .Where(c => c.Id == id && c.IsActive == true)
                .Select(c => new MatchDto
                {
                    Id = c.Id,
                    Player1Id = c.Player1Id,
                    Player1Name = c.Player1.Name,
                    Player2Id = c.Player2Id,
                    Player2Name = c.Player2.Name,
                    BestOfSets = c.BestOfSets,
                    MatchDate = c.MatchDate,
                    Player1Score = c.Player1Score,
                    Player2Score = c.Player2Score,
                    IsFinished = c.IsFinished
                })
                .FirstOrDefault();

            return match;
        }

        public void DeleteMatch(int id)
        {
            var match = _dbContext.Matches.FirstOrDefault(c => c.Id == id);
            if (match != null)
            {
                match.IsActive = false;
                _dbContext.SaveChanges();
            }
        }

        public PagedResult<MatchDto> ReadMatches(string sortColumn, string sortOrder, string q, int pageNo)
        {
            var query = _dbContext.Matches
                .Where(c => c.IsActive == true)
                .Select(c => new MatchDto
                {
                    Id = c.Id,
                    Player1Id = c.Player1Id,
                    Player1Name = c.Player1.Name,
                    Player2Id = c.Player2Id,
                    Player2Name = c.Player2.Name,
                    BestOfSets = c.BestOfSets,
                    MatchDate = c.MatchDate,
                    Player1Score = c.Player1Score,
                    Player2Score = c.Player2Score,
                    IsFinished = c.IsFinished
                });

            if (!string.IsNullOrEmpty(q))
            {
                query = query
                    .Where(c => c.Id.ToString().Contains(q)
                    || c.Player1Name.Contains(q)
                    || c.Player2Name.Contains(q)
                    || c.MatchDate.ToString().Contains(q));
            }

            if (sortColumn == "Id")
                if (sortOrder == "asc")
                    query = query.OrderBy(s => s.Id);
                else if (sortOrder == "desc")
                    query = query.OrderByDescending(s => s.Id);

            if (sortColumn == "Player1Name")
                if (sortOrder == "asc")
                    query = query.OrderBy(s => s.Player1Name);
                else if (sortOrder == "desc")
                    query = query.OrderByDescending(s => s.Player1Name);

            if (sortColumn == "Player2Name")
                if (sortOrder == "asc")
                    query = query.OrderBy(s => s.Player2Name);
                else if (sortOrder == "desc")
                    query = query.OrderByDescending(s => s.Player2Name);

            if (sortColumn == "MatchDate")
                if (sortOrder == "asc")
                    query = query.OrderBy(s => s.MatchDate);
                else if (sortOrder == "desc")
                    query = query.OrderByDescending(s => s.MatchDate);

            if ( sortColumn == "BestOfSets")
                if (sortOrder == "asc")
                    query = query.OrderBy(s => s.BestOfSets);
                else if (sortOrder == "desc")
                    query = query.OrderByDescending(s => s.BestOfSets);
            return query.GetPaged(pageNo, 10);
        }
    }
}
