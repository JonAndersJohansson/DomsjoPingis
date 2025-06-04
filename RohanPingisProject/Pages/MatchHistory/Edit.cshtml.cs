using DataAccessLayer.Data.Models;
using DataAccessLayer.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Interface;
using System.ComponentModel.DataAnnotations;

namespace RohanPingisProject.Pages.Match
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IMatchService _matchService;

        public EditModel(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [BindProperty]
        public EditMatchViewModel Input { get; set; }

        public List<SelectListItem> PlayerList { get; set; }
        public List<SelectListItem> SetOptions { get; set; }

        public class EditMatchViewModel
        {
            public int Id { get; set; }

            [Required]
            [Display(Name = "Spelare 1")]
            public int Player1Id { get; set; }

            [Required]
            [Display(Name = "Spelare 2")]
            public int Player2Id { get; set; }

            [Required]
            [Display(Name = "Bäst av antal set")]
            public BestOf BestOf { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "Matchdatum och tid")]
            public DateTime MatchDate { get; set; }

            [Range(0, int.MaxValue)]
            [Display(Name = "Spelare 1 – poäng")]
            public int Player1Score { get; set; }

            [Range(0, int.MaxValue)]
            [Display(Name = "Spelare 2 – poäng")]
            public int Player2Score { get; set; }

            [Display(Name = "Match färdigspelad")]
            public bool IsFinished { get; set; }

            public List<EditSetViewModel> Sets { get; set; } = new();
        }

        public class EditSetViewModel
        {
            public int Id { get; set; }
            public int SetNumber { get; set; }

            [Range(0, int.MaxValue)]
            [Display(Name = "P1 poäng")]
            public int Player1Points { get; set; }

            [Range(0, int.MaxValue)]
            [Display(Name = "P2 poäng")]
            public int Player2Points { get; set; }

            [Display(Name = "Avslutat")]
            public bool IsFinished { get; set; }
        }

        public IActionResult OnGet(int id)
        {
            var dto = _matchService.GetById(id);
            if (dto == null) return NotFound();

            PlayerList = _matchService.GetPlayerList();
            SetOptions = _matchService.GetSetList();

            Input = new EditMatchViewModel
            {
                Id = dto.Id,
                Player1Id = dto.Player1Id,
                Player2Id = dto.Player2Id,
                BestOf = dto.BestOfSets,
                MatchDate = dto.MatchDate,
                Player1Score = dto.Player1Score,
                Player2Score = dto.Player2Score,
                IsFinished = dto.IsFinished,

                Sets = dto.Sets
                         .OrderBy(s => s.SetNumber)
                         .Select(s => new EditSetViewModel
                         {
                             Id = s.Id,
                             SetNumber = s.SetNumber,
                             Player1Points = s.Player1Points,
                             Player2Points = s.Player2Points,
                             IsFinished = s.IsFinished
                         })
                         .ToList()
            };

            var maxSets = (int)dto.BestOfSets;
            for (int setNum = 1; setNum <= maxSets; setNum++)
            {
                if (!Input.Sets.Any(s => s.SetNumber == setNum))
                {
                    Input.Sets.Add(new EditSetViewModel { SetNumber = setNum });
                }
            }
            Input.Sets = Input.Sets.OrderBy(s => s.SetNumber).ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            PlayerList = _matchService.GetPlayerList();
            SetOptions = _matchService.GetSetList();

            var originalMatch = _matchService.GetById(Input.Id);
            if (originalMatch == null) return NotFound();

            if (Input.Player1Id == Input.Player2Id)
                ModelState.AddModelError("Input.Player1Id", "Spelare 1 och 2 får inte vara samma");

            var player1SetsWon = 0;
            var player2SetsWon = 0;
            var maxSets = (int)originalMatch.BestOfSets;
            var setsToWin = maxSets / 2 + 1;
            var matchDecidedAtSet = -1; 
            var scoreWhenDecided = ""; 

            for (int i = 0; i < Input.Sets.Count; i++)
            {
                var set = Input.Sets[i];

                if (set.Player1Points > 0 || set.Player2Points > 0)
                {
                    if ((set.Player1Points >= 11 || set.Player2Points >= 11) &&
                        Math.Abs(set.Player1Points - set.Player2Points) >= 2)
                    {
                        if (set.Player1Points > set.Player2Points)
                            player1SetsWon++;
                        else if (set.Player2Points > set.Player1Points)
                            player2SetsWon++;

                        if (matchDecidedAtSet == -1 && (player1SetsWon >= setsToWin || player2SetsWon >= setsToWin))
                        {
                            matchDecidedAtSet = i + 1; 
                            scoreWhenDecided = $"{player1SetsWon}-{player2SetsWon}";
                        }
                    }
                }
            }

            player1SetsWon = 0;
            player2SetsWon = 0;

            for (int i = 0; i < Input.Sets.Count; i++)
            {
                var set = Input.Sets[i];

                if (matchDecidedAtSet != -1 && set.SetNumber > matchDecidedAtSet &&
                    (set.Player1Points > 0 || set.Player2Points > 0))
                {
                    ModelState.AddModelError($"Input.Sets[{i}].Player1Points",
                        $"Set {set.SetNumber} kan inte ha poäng eftersom matchen redan var avgjord {scoreWhenDecided} efter set {matchDecidedAtSet} (bäst av {maxSets})");
                    continue; 
                }

                
                if (set.Player1Points > 0 || set.Player2Points > 0)
                {
                    if (set.Player1Points >= 10 && set.Player2Points >= 10)
                    {
                        var pointDiff = Math.Abs(set.Player1Points - set.Player2Points);

                        if (pointDiff > 2)
                        {
                            ModelState.AddModelError($"Input.Sets[{i}].Player1Points",
                                $"Set {set.SetNumber}: När båda spelarna har nått 10 poäng måste setet vinnas med exakt 2 poängs marginal. " +
                                $"Poängställningen {set.Player1Points}-{set.Player2Points} är inte tillåten.");
                        }
                    }

                    if (set.IsFinished && set.Player1Points < 11 && set.Player2Points < 11)
                    {
                        ModelState.AddModelError($"Input.Sets[{i}].Player1Points",
                            $"Set {set.SetNumber}: Ett set kan inte vara avslutat om ingen spelare har nått 11 poäng");
                    }
                    if (set.Player1Points >= 11 || set.Player2Points >= 11)
                    {
                        var pointDiff = Math.Abs(set.Player1Points - set.Player2Points);

                        if (pointDiff < 2)
                        {
                            if (set.IsFinished)
                            {
                                ModelState.AddModelError($"Input.Sets[{i}].Player1Points",
                                    $"Set {set.SetNumber}: Ett avslutat set måste ha minst 2 poängs skillnad");
                            }
                        }
                        else
                        {
                            if (!set.IsFinished)
                            {
                                ModelState.AddModelError($"Input.Sets[{i}].IsFinished",
                                    $"Set {set.SetNumber}: Detta set borde vara markerat som avslutat");
                            }
                        }
                    }
                    if ((set.Player1Points > 11 && set.Player2Points < 9) ||
                        (set.Player2Points > 11 && set.Player1Points < 9))
                    {
                        ModelState.AddModelError($"Input.Sets[{i}].Player1Points",
                            $"Set {set.SetNumber}: Orimlig poängkombination enligt pingisregler");
                    }
                    if (set.IsFinished)
                    {
                        if (set.Player1Points > set.Player2Points)
                            player1SetsWon++;
                        else if (set.Player2Points > set.Player1Points)
                            player2SetsWon++;
                    }

                    else if ((set.Player1Points >= 11 || set.Player2Points >= 11) &&
                             Math.Abs(set.Player1Points - set.Player2Points) >= 2)
                    {
                        if (set.Player1Points > set.Player2Points)
                            player1SetsWon++;
                        else if (set.Player2Points > set.Player1Points)
                            player2SetsWon++;
                    }
                }
            }

            var finalPlayer1SetsWon = 0;
            var finalPlayer2SetsWon = 0;

            for (int i = 0; i < Input.Sets.Count; i++)
            {
                var set = Input.Sets[i];

                if (matchDecidedAtSet != -1 && set.SetNumber > matchDecidedAtSet)
                    break;

                if ((set.Player1Points >= 11 || set.Player2Points >= 11) &&
                    Math.Abs(set.Player1Points - set.Player2Points) >= 2)
                {
                    if (set.Player1Points > set.Player2Points)
                        finalPlayer1SetsWon++;
                    else if (set.Player2Points > set.Player1Points)
                        finalPlayer2SetsWon++;
                }
            }

            if (Input.IsFinished)
            {
                if (finalPlayer1SetsWon < setsToWin && finalPlayer2SetsWon < setsToWin)
                {
                    ModelState.AddModelError("Input.IsFinished",
                        $"Matchen kan inte vara avslutad. Någon måste vinna {setsToWin} set i en bäst av {maxSets} match");
                }
            }
            if (Input.Player1Score != finalPlayer1SetsWon || Input.Player2Score != finalPlayer2SetsWon)
            {
                ModelState.AddModelError("Input.Player1Score",
                    $"Matchpoängen stämmer inte överens med antalet vunna set. " +
                    $"Spelare 1 har vunnit {finalPlayer1SetsWon} set, Spelare 2 har vunnit {finalPlayer2SetsWon} set");
            }
            if (Input.MatchDate > DateTime.Now)
            {
                ModelState.AddModelError("Input.MatchDate", "Matchdatum kan inte vara i framtiden");
            }

            if (!ModelState.IsValid)
                return Page();

            player1SetsWon = 0;
            player2SetsWon = 0;

            var dto = new MatchDto
            {
                Id = Input.Id,
                Player1Id = Input.Player1Id,
                Player2Id = Input.Player2Id,
                MatchDate = Input.MatchDate,
                BestOfSets = originalMatch.BestOfSets,
                Player1Score = 0,  
                Player2Score = 0,  
                IsFinished = Input.IsFinished,

                Sets = Input.Sets?.Select(s =>
                {
                    if (matchDecidedAtSet != -1 && s.SetNumber > matchDecidedAtSet)
                    {
                        return new SetDto
                        {
                            Id = s.Id,
                            MatchId = Input.Id,
                            SetNumber = s.SetNumber,
                            Player1Points = 0,
                            Player2Points = 0,
                            IsFinished = false
                        };
                    }

                    var shouldBeFinished = false;
                    if ((s.Player1Points >= 11 || s.Player2Points >= 11) &&
                        Math.Abs(s.Player1Points - s.Player2Points) >= 2)
                    {
                        shouldBeFinished = true;

                        if (s.Player1Points > s.Player2Points)
                            player1SetsWon++;
                        else
                            player2SetsWon++;
                    }

                    return new SetDto
                    {
                        Id = s.Id,
                        MatchId = Input.Id,
                        SetNumber = s.SetNumber,
                        Player1Points = s.Player1Points,
                        Player2Points = s.Player2Points,
                        IsFinished = shouldBeFinished
                    };
                }).ToList()
            };

            dto.Player1Score = player1SetsWon;
            dto.Player2Score = player2SetsWon;

            _matchService.UpdateMatch(dto);
            return RedirectToPage("/MatchHistory/MatchDetails", new { id = dto.Id });
        }
    }
}