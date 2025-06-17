using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using DataAccessLayer.DTO_s;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Infrastructure.Translator;
using Service.Interface;

namespace Service.Service
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public PlayerService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void CreateNewPlayer(PlayerDto playerDto)
        {
            var newPlayer = new Player
            {
                Name = playerDto.Name,
                Gender = playerDto.Gender,
                BirthDate = playerDto.BirthDate
            };

            _dbContext.Players.Add(newPlayer);
            _dbContext.SaveChanges();

            playerDto.Id = newPlayer.Id;
        }

        public List<SelectListItem> GetGenderList()
        {
            return Enum.GetValues<Gender>()
                .Select(g => new SelectListItem
                {
                    Value = ((int)g).ToString(),
                    Text = GenderTranslator.Translate(g)
                }).ToList();
        }

        public PlayerDto GetPlayerById(int id)
        {
            var player = _dbContext.Players.FirstOrDefault(p => p.Id == id);

            if (player == null)
            {
                return null;
            }

            return new PlayerDto
            {
                Id = player.Id,
                Name = player.Name,
                Gender = player.Gender,
                BirthDate = player.BirthDate
            };
        }



        public Validation ConfirmInput (int player1, int player2)
        {
            var p1 = _dbContext.Players.Find(player1);
            var p2 = _dbContext.Players.Find(player2);

            if (p1 == null)
            {
                return Validation.P1NotFound;
            }
            else if (p2 == null)
            {
                return Validation.P2NotFound;
            }
            else if (p1.Id == p2.Id)
            {
                return Validation.P1SameAsP2;
            }
            else
            {
                return Validation.OK;
            }
        }

        public IEnumerable<PlayerAutocompleteDto> SearchPlayers(string term)
        {
            return _dbContext.Players
                .Where(p => p.IsActive && p.Name.Contains(term))
                .OrderBy(p => p.Name)
                .Select(p => new PlayerAutocompleteDto
                {
                    Id = p.Id,
                    DisplayName = $"{p.Name} {p.BirthDate:yy-MM-dd}"
                })
                .ToList();
        }

    }
}
