using AutoMapper;
using DataAccessLayer.Data.Models;
using DataAccessLayer.DTO_s;
using DomsjoPingisProject.ViewModels;

public class PingisProfile : Profile
{
    public PingisProfile()
    {
        CreateMap<PlayerDto, PlayerViewModel>().ReverseMap();
        CreateMap<Player, PlayerViewModel>().ReverseMap();
        CreateMap<SetDto, SetViewModel>().ReverseMap();
        CreateMap<MatchDto, MatchViewModel>().ReverseMap();
        CreateMap<PlayerStatisticsDto, PlayerStatisticsViewModel>().ReverseMap();
    }
}
