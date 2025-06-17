using Microsoft.AspNetCore.Mvc.RazorPages;
using DomsjoPingisProject.ViewModels;
using Service.Interface;

namespace DomsjoPingisProject.Pages.MatchHistory
{
    public class TopTenModel : PageModel
    {
        private readonly ITopTenService _topTenService;

        public TopTenModel(ITopTenService topTenService)
        {
            _topTenService = topTenService;
        }

        public List<TopPlayerViewModel> TopBySets { get; set; }
        public List<TopPlayerViewModel> TopByWinRatio { get; set; }

        public List<TopMatchViewModel> LongestMatches { get; set; }
        public List<TopMatchViewModel> ShortestMatches { get; set; }

        public void OnGet()
        {
            var topSetsDto = _topTenService.GetTopBySets();
            var topWinsDto = _topTenService.GetTopByWinRatio();

            TopBySets = topSetsDto.Select(dto => new TopPlayerViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                TotalSetsWon = dto.TotalSetsWon,
                MatchesWon = dto.MatchesWon
            }).ToList();

            TopByWinRatio = topWinsDto.Select(dto => new TopPlayerViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                TotalSetsWon = dto.TotalSetsWon,
                MatchesWon = dto.MatchesWon,
                MatchesPlayed = dto.MatchesPlayed,
                TopByWinRatio = dto.WinRatio
            }).ToList();

            LongestMatches = _topTenService.GetLongestMatches()
            .Select(dto => new TopMatchViewModel
            {
                MatchId = dto.MatchId,
                Player1Name = dto.Player1Name,
                Player2Name = dto.Player2Name,
                TotalDuration = dto.TotalDuration,
                MatchDate = dto.MatchDate
            }).ToList();

            ShortestMatches = _topTenService.GetShortestMatches()
                .Select(dto => new TopMatchViewModel
                {
                    MatchId = dto.MatchId,
                    Player1Name = dto.Player1Name,
                    Player2Name = dto.Player2Name,
                    TotalDuration = dto.TotalDuration,
                    MatchDate = dto.MatchDate
                }).ToList();

        }
    }
}
