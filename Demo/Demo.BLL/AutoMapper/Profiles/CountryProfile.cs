using AutoMapper;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Models.DTOModels.Country;

namespace Demo.BLL.AutoMapper.Profiles;

public class CountryProfile : Profile
{
    public CountryProfile()
    {
        CreateMap<Country, CountryDto>();
    }
}