using AutoMapper;
using Demo.BLL.Features.Resource.Commands.Create;
using Demo.BLL.Features.Resource.Commands.Update;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Models.DTOModels.Resource;

namespace Demo.BLL.AutoMapper.Profiles;

public class ResourceProfile : Profile
{
    public ResourceProfile()
    {
        CreateMap<Resource, ResourceDto>();

        CreateMap<CreateCommand, Resource>();

        CreateMap<UpdateCommand, Resource>()
            .ForMember(dest => dest.Id, src => src.Ignore());
    }
}