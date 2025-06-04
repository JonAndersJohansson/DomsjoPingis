using DataAccessLayer.DTO_s;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Service.Interface
{
    public interface IPlayerService
    {
        Validation ConfirmInput(int player1, int player2);
        public void CreateNewPlayer(PlayerDto playerDto);
        List<SelectListItem> GetGenderList();
        public PlayerDto GetPlayerById(int id);
        public List<object> SearchPlayers(string term);

    }



    public enum Validation
    {
        OK,
        P1NotFound,
        P2NotFound,
        P1SameAsP2,
    }
}
