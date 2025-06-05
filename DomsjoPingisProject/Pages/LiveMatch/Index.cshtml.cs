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
        [Required(ErrorMessage = "Välj antal set")]
        public BestOf BestOfValue { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Id för Spelare 1 krävs")]
        public int Player1Id { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Id för Spelare 2 krävs")]
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
                    TempData["Error"] = "Kunde inte hitta spelare 1, försök igen";
                    return RedirectToPage("Index");
                }
                else if (status == Validation.P2NotFound)
                {
                    TempData["Error"] = "Kunde inte hitta spelare 2, försök igen";
                    return RedirectToPage("Index");
                }
                else if (status == Validation.P1SameAsP2)
                {
                    TempData["Error"] = "Spelare kan inte möta sig själv.";
                    return RedirectToPage("Index");
                }
                else
                {
                    var matchId = _matchService.CreateGame(Player1Id, Player2Id, BestOfValue);
                    return RedirectToPage("Game", new { matchId });
                }

            }
            TempData["Error"] = "Ogiltigt värde, försök igen.";
            return RedirectToPage("Index");
        }
    }
}
