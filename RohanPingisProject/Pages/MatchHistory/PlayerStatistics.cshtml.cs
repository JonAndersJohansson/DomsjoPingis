using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomsjoPingisProject.ViewModels;
using Service.Interface;
using System.ComponentModel.DataAnnotations;

namespace DomsjoPingisProject.Pages.MatchHistory
{
    [Authorize(Roles = "Admin")]
    public class PlayerStatisticsModel : PageModel
    {
        private readonly IPlayerStatisticsService _playerStatisticsService;
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;

        public PlayerStatisticsModel(IPlayerStatisticsService playerStatisticsService,
            IMapper mapper, IPlayerService playerService)
        {
            _playerStatisticsService = playerStatisticsService;
            _mapper = mapper;
            _playerService = playerService;
        }

        public PlayerStatisticsViewModel PlayerStatistics { get; set; }
        public PlayerStatisticsViewModel HeadToHeadStats { get; set; }
        public PlayerStatisticsViewModel PlayerBestAndWorst { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Id för Spelare 1 krävs")]
        public int Player1Id { get; set; }

        [BindProperty]
        public string Player1NameStats { get; set; }

        [BindProperty]
        public int Player2Id { get; set; }

        [BindProperty]
        public string Player2NameHeadToHead { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostPlayerStatistics()
        {
            LoadAllPlayer1Stats();
            return Page();
        }

        public IActionResult OnPostHeadToHead()
        {
            LoadAllPlayer1Stats();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Ogiltigt värde, försök igen.";
                return Page();
            }

            if (Player1Id == Player2Id)
            {
                TempData["Error"] = "Du kan inte välja samma spelare som både huvudspelare och motståndare.";
                return Page();
            }

            var headToHeadDto = _playerStatisticsService.GetHeadToHeadStats(Player1Id, Player2Id);
            if (headToHeadDto == null)
            {
                TempData["Error"] = "Ingen statistik mellan dessa spelare.";
                return Page();
            }

            HeadToHeadStats = _mapper.Map<PlayerStatisticsViewModel>(headToHeadDto);

            var opponent = _playerService.GetPlayerById(Player2Id);
            Player2NameHeadToHead = opponent?.Name ?? "";

            return Page();
        }

        private void LoadAllPlayer1Stats()
        {
            var playerStatisticsDto = _playerStatisticsService.GetStatisticsForPlayer(Player1Id);
            PlayerStatistics = _mapper.Map<PlayerStatisticsViewModel>(playerStatisticsDto);

            var bestAndWorstDto = _playerStatisticsService.PlayerBestAndWorst(Player1Id);
            PlayerBestAndWorst = _mapper.Map<PlayerStatisticsViewModel>(bestAndWorstDto);

            Player1NameStats = _playerService.GetPlayerById(Player1Id)?.Name ?? "";
        }
    }
}