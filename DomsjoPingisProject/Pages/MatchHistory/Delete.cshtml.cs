using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;

namespace DomsjoPingisProject.Pages.MatchHistory
{
    [Authorize(Roles = "Admin")]

    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly IMatchHistoryService _matchService;

        public DeleteModel(IMatchHistoryService matchService)
        {
            _matchService = matchService;
        }

        public int MatchId { get; set; }

        public IActionResult OnGet(int id)
        {
            var match = _matchService.GetMatchById(id);
            if (match == null)
            {
                return RedirectToPage("Index");
            }
            MatchId = match.Id;
            return Page();
        }
        public IActionResult OnPost(int id)
        {
            if (ModelState.IsValid)
            {
                MatchId = id;
                var customer = _matchService.GetMatches(MatchId);
                if (customer == null)
                {
                    return NotFound();
                }

                _matchService.DeleteMatch(MatchId);
                return RedirectToPage("/MatchHistory/Index");
            }
            return Page();
        }
    }
}
