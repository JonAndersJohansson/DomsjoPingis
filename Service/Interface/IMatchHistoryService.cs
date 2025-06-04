using DataAccessLayer.DTO_s;
using Service.Infrastructure.Paging;

namespace Service.Interface
{
    public interface IMatchHistoryService
    {
        List<MatchDto> GetMatches(int id);
        MatchDto GetMatchById(int id);
        void DeleteMatch(int id);
        PagedResult<MatchDto> ReadMatches(string sortColumn, string sortOrder,
            string q, int pageNo);
    }
}
