using AutoMapper;
using DataAccessLayer.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RohanPingisProject.ViewModels;
using Service.Interface;

namespace RohanPingisProject.Pages.LiveMatch
{
    public class GameModel : PageModel
    {
        private readonly IMatchService _matchService;
        private readonly IMapper _mapper;

        public GameModel(IMatchService matchService, IMapper mapper)
        {
            _matchService = matchService;
            _mapper = mapper;
        }

        [BindProperty(SupportsGet = true)]
        public int MatchId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ShowSetWinBanner { get; set; }

        [BindProperty]
        public SetViewModel CurrentSet { get; set; }

        [BindProperty]
        public string Winner { get; set; }

        public MatchViewModel? Match { get; set; }
        public SetViewModel? LastFinishedSet { get; set; }

        [TempData]
        public bool IsCorrection { get; set; }

        public IActionResult OnGet()
        {
            if (TempData.ContainsKey("IsCorrection"))
            {
                TempData.Keep("IsCorrection");
                IsCorrection = Convert.ToBoolean(TempData["IsCorrection"]);
            }

            var matchDto = _matchService.GetById(MatchId);
            if (matchDto == null)
            {
                TempData["NoGame"] = "Kunde inte hitta skapad match.";
                return RedirectToPage("Index");
            }

            Match = _mapper.Map<MatchViewModel>(matchDto);

            if (Match.IsFinished)
            {
                Match.MatchDuration = Match.Sets
                    .Where(s => s.SetDuration.HasValue)
                    .Aggregate(TimeSpan.Zero, (sum, s) => sum + s.SetDuration.GetValueOrDefault());

                Winner = Match.Player1Score > Match.Player2Score
                    ? Match.Player1Name
                    : Match.Player2Name;
                return Page();
            }

            var setDto = _matchService.GetCurrentSet(MatchId);
            if (setDto == null)
            {
                TempData["NoSet"] = $"Kunde inte hitta set kopplat till MatchId: {MatchId}.";
                return RedirectToPage("Index");
            }

            bool isMatchStart = setDto.SetNumber == 1 && setDto.Player1Points == 0 && setDto.Player2Points == 0;

            if (isMatchStart && !Match.IsP1FirstServer.HasValue)
            {
                ViewData["ShowFirstServerModal"] = true;
            }

            setDto = _matchService.UpdateSetStatusFlags(setDto, matchDto);
            CurrentSet = _mapper.Map<SetViewModel>(setDto);

            var lastFinished = Match.Sets
                .Where(s => s.IsFinished)
                .OrderByDescending(s => s.SetNumber)
                .FirstOrDefault();

            if (ShowSetWinBanner.HasValue && lastFinished != null && lastFinished.SetNumber == ShowSetWinBanner.Value)
            {
                lastFinished.WinnerFirstName = lastFinished.Player1Points > lastFinished.Player2Points
                    ? Match.Player1FirstName
                    : Match.Player2FirstName;
                LastFinishedSet = lastFinished;
            }
            else
            {
                LastFinishedSet = null;
            }

            return Page();
        }

        public IActionResult OnPostAddPointToPlayer1()
        {
            var matchBefore = _matchService.GetById(MatchId);
            int finishedSetsBefore = matchBefore.Sets.Count(s => s.IsFinished);

            _matchService.HandlePoint(MatchId, 1, true);

            var matchAfter = _matchService.GetById(MatchId);
            int finishedSetsAfter = matchAfter.Sets.Count(s => s.IsFinished);

            if (finishedSetsAfter > finishedSetsBefore)
            {
                var lastFinishedSet = matchAfter.Sets
                    .Where(s => s.IsFinished)
                    .OrderByDescending(s => s.SetNumber)
                    .FirstOrDefault();

                if (lastFinishedSet != null)
                    return RedirectToPage(new { matchId = MatchId, showSetWinBanner = lastFinishedSet.SetNumber });
            }

            return RedirectToPage(new { matchId = MatchId });
        }

        public IActionResult OnPostAddPointToPlayer2()
        {
            var matchBefore = _matchService.GetById(MatchId);
            int finishedSetsBefore = matchBefore.Sets.Count(s => s.IsFinished);

            _matchService.HandlePoint(MatchId, 2, true);

            var matchAfter = _matchService.GetById(MatchId);
            int finishedSetsAfter = matchAfter.Sets.Count(s => s.IsFinished);

            if (finishedSetsAfter > finishedSetsBefore)
            {
                var lastFinishedSet = matchAfter.Sets
                    .Where(s => s.IsFinished)
                    .OrderByDescending(s => s.SetNumber)
                    .FirstOrDefault();

                if (lastFinishedSet != null)
                    return RedirectToPage(new { matchId = MatchId, showSetWinBanner = lastFinishedSet.SetNumber });
            }

            return RedirectToPage(new { matchId = MatchId });
        }

        public IActionResult OnPostWithdrawPointFromPlayer1()
        {
            _matchService.HandlePoint(MatchId, 1, false);
            return RedirectToPage(new { matchId = MatchId });
        }

        public IActionResult OnPostWithdrawPointFromPlayer2()
        {
            _matchService.HandlePoint(MatchId, 2, false);
            return RedirectToPage(new { matchId = MatchId });
        }

        public IActionResult OnPostStartRematch(int matchId)
        {
            var matchDto = _matchService.GetById(matchId);
            var newMatchId = _matchService.CreateGame(matchDto.Player1Id, matchDto.Player2Id, matchDto.BestOfSets);
            return RedirectToPage("Game", new { matchId = newMatchId });
        }

        public IActionResult OnPostStartNewMatch(int player1Id, int player2Id, BestOf bestOf)
        {
            var newMatchId = _matchService.CreateGame(player1Id, player2Id, bestOf);
            return RedirectToPage("Game", new { matchId = newMatchId });
        }
        public IActionResult OnPostCorrection()
        {
            IsCorrection = !IsCorrection;
            return RedirectToPage(new { matchId = MatchId });
        }
        public IActionResult OnPostSetFirstServer()
        {
            var firstServerValue = Request.Form["Match.IsP1FirstServer"];
            if (!bool.TryParse(firstServerValue, out bool isP1FirstServer))
            {
                ModelState.AddModelError("Match.IsP1FirstServer", "Du måste välja en servare.");
            }

            var matchDto = _matchService.GetById(MatchId);
            if (matchDto == null)
            {
                TempData["NoGame"] = "Kunde inte hitta skapad match.";
                return RedirectToPage("Index");
            }
            Match = _mapper.Map<MatchViewModel>(matchDto);

            matchDto.IsP1FirstServer = isP1FirstServer;
            _matchService.UpdateMatch(matchDto);

            return RedirectToPage(new { matchId = MatchId });
        }
    }
}
