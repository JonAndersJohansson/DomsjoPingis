using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Interface;
using System.ComponentModel.DataAnnotations;

namespace DomsjoPingisProject.Pages.LiveMatch
{
    public class IndexModel : PageModel
    {
        private readonly IMatchService _matchService;
        private readonly IPlayerService _playerService;

        public IndexModel(IMatchService matchService, IPlayerService playerService)
        {
            _matchService = matchService;
            _playerService = playerService;
        }

        [BindProperty]
        [Required(ErrorMessage = "V�lj antal set")]
        public BestOf BestOfValue { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Id f�r Spelare 1 kr�vs")]
        public int Player1Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Id f�r Spelare 2 kr�vs")]
        public int Player2Id { get; set; }

        public List<SelectListItem> BestOf { get; set; } = new();


        public void OnGet()
        {
            BestOf = _matchService.GetSetList();    
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var status = _playerService.ConfirmInput(Player1Id, Player2Id);
                if (status == Validation.P1NotFound)
                {
                    TempData["Error"] = "Kunde inte hitta spelare 1, f�rs�k igen";
                    return RedirectToPage("Index");
                }
                else if (status == Validation.P2NotFound)
                {
                    TempData["Error"] = "Kunde inte hitta spelare 2, f�rs�k igen";
                    return RedirectToPage("Index");
                }
                else if (status == Validation.P1SameAsP2)
                {
                    TempData["Error"] = "Spelare kan inte m�ta sig sj�lv.";
                    return RedirectToPage("Index");
                }
                else
                {
                    var matchId = _matchService.CreateGame(Player1Id, Player2Id, BestOfValue);
                    return RedirectToPage("Game", new { matchId });
                }

            }
            TempData["Error"] = "Ogiltigt v�rde, f�rs�k igen.";
            return RedirectToPage("Index");
        }
    }
}
