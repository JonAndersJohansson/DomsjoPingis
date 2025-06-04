using DataAccessLayer.Data.Models;
using DataAccessLayer.DTO_s;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Service.Interface
{
    public interface IMatchService
    {
        List<SelectListItem>? GetSetList();
        int CreateGame(int player1Id, int player2Id, BestOf BestOfValue);
        MatchDto GetById(int matchId);
        void StartNewSet(int matchId);
        void HandlePoint(int matchId, int player, bool isToAdd);
        SetDto GetCurrentSet(int matchId);
        SetDto UpdateSetStatusFlags(SetDto setDto, MatchDto matchDto);
        void UpdateMatch(MatchDto updatedMatch);
        List<SelectListItem> GetPlayerList();
    }
}
