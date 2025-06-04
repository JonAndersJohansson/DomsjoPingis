using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interface;

namespace DomsjoPingisProject.Pages.Api
{
    public class SearchPlayersModel : PageModel
    {
        private readonly IPlayerService _playerService;

        public SearchPlayersModel(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public JsonResult OnGet(string term)
        {
            var results = _playerService.SearchPlayers(term ?? "");
            return new JsonResult(results);
        }
    }
}

