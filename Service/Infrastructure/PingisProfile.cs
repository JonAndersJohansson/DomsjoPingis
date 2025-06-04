using AutoMapper;
using DataAccessLayer.Data.Models;
using DataAccessLayer.DTO_s;

namespace Service.Infrastructure
{
    public class PingisProfile : Profile
    {
        public PingisProfile()
        {
            CreateMap<Player, PlayerDto>().ReverseMap();
            CreateMap<Set, SetDto>().ReverseMap();
            CreateMap<Match, MatchDto>().ReverseMap();
        }
    }
}

