using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RohanPingisProject.ViewModels;
using Service.Interface;

namespace RohanPingisProject.Pages.Player
{
    public class ConfirmPlayerModel : PageModel
    {
        private readonly IPlayerService _playerService;

        public ConfirmPlayerModel(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public PlayerViewModel PlayerInfo { get; set; }

        public IActionResult OnGet(int id)
        {
            var playerDetails = _playerService.GetPlayerById(id);
            if (playerDetails == null)
                return RedirectToPage("/Index");

            PlayerInfo = new PlayerViewModel
            {
                Id = playerDetails.Id,
                Name = playerDetails.Name,
                Gender = playerDetails.Gender,
                BirthDate = playerDetails.BirthDate
            };

            return Page();
        }
    }
}
