using DataAccessLayer.DTO_s;

namespace Service.Interface
{
    public interface IPlayerStatisticsService
    {
        Validation ConfirmInput(int player1);
        PlayerStatisticsDto GetStatisticsForPlayer(int player1Id);
        PlayerStatisticsDto PlayerBestAndWorst(int player1Id);
        PlayerStatisticsDto GetHeadToHeadStats(int player1Id, int player2Id);
    }
}
