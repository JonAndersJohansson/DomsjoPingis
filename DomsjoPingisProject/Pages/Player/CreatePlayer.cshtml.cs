using AutoMapper;
using DataAccessLayer.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DomsjoPingisProject.ViewModels;
using Service.Interface;

namespace DomsjoPingisProject.Pages.Player
{
    public class CreatePlayerModel : PageModel
    {
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;

        public CreatePlayerModel(IPlayerService playerService, IMapper mapper)
        {
            _playerService = playerService;
            _mapper = mapper;
        }

        [BindProperty]
        public PlayerViewModel NewPlayer { get; set; }

        public List<SelectListItem> GenderOptions { get; set; }


        public void OnGet()
        {
            GenderOptions = _playerService.GetGenderList();
        }

        public IActionResult OnPost()
        {
            if (NewPlayer.BirthDate > DateOnly.FromDateTime(DateTime.Now))
            { 
                ModelState.AddModelError("NewPlayer.BirthDate", "Födelsedatum kan inte vara i framtiden");
            }

            if (!ModelState.IsValid)
            {
                GenderOptions = _playerService.GetGenderList();
                return Page();
            }

            var newPlayer = _mapper.Map<PlayerDto>(NewPlayer);
            _playerService.CreateNewPlayer(newPlayer);


            return RedirectToPage("ConfirmPlayer", new { id = newPlayer.Id });

        }
    }
}
