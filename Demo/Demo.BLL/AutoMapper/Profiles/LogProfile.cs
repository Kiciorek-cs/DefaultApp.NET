using AutoMapper;
using Demo.Domain.Models.DTOModels.Log;

namespace Demo.BLL.AutoMapper.Profiles
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<Demo.Domain.Entities.Demo.Log, LogDto>()
                .ReverseMap();

        }
    }
}

