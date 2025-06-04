using DataAccessLayer.DTOs;

namespace Service.Interface
{
    public interface ITopTenService
    {
        List<TopPlayerDto> GetTopBySets();
        List<TopPlayerDto> GetTopByWins();
        List<TopMatchDto> GetLongestMatches(int count = 10);
        List<TopMatchDto> GetShortestMatches(int count = 10);
    }

}
