using DataAccessLayer.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;

namespace RohanPingisProject.Pages.MatchDetails
{
    [Authorize(Roles = "Admin")]

    [BindProperties]
    public class MatchDetailsModel : PageModel
    {
        private readonly IMatchService _matchService;
        private readonly IMatchHistoryService _historyService;

        public MatchDto Match { get; set; } = default!;

        public MatchDetailsModel(
            IMatchService matchService,
            IMatchHistoryService historyService)
        {
            _matchService = matchService;
            _historyService = historyService;
        }

        public IActionResult OnGet(int id)
        {
            Match = _matchService.GetById(id);
            if (Match == null) return NotFound();

            if (Match.IsFinished && Match.Sets != null && Match.Sets.All(s => s.SetDuration.HasValue))
            {
                var totalTicks = Match.Sets
                    .Where(s => s.SetDuration.HasValue)
                    .Sum(s => s.SetDuration.Value.Ticks);

                Match.MatchDuration = TimeSpan.FromTicks(totalTicks);
            }

            var maxSets = (int)Match.BestOfSets;
            for (int setNum = 1; setNum <= maxSets; setNum++)
            {
                if (!Match.Sets.Any(s => s.SetNumber == setNum))
                {
                    Match.Sets.Add(new SetDto
                    {
                        SetNumber = setNum,
                        Player1Points = 0,
                        Player2Points = 0,
                        IsFinished = false
                    });
                }
            }
            Match.Sets = Match.Sets.OrderBy(s => s.SetNumber).ToList();


            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            _historyService.DeleteMatch(id);
            return RedirectToPage("/MatchHistory/Index");
        }
    }
}
