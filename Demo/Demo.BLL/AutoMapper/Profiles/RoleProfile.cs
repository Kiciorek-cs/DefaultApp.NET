using System.Linq;
using AutoMapper;
using Demo.BLL.Features.Role.Commands.Create;
using Demo.BLL.Features.Role.Commands.Update;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Models.DTOModels.Role;

namespace Demo.BLL.AutoMapper.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Resources, src => src.MapFrom(x => x.Resources.Select(y => y.Id)));

        CreateMap<CreateCommand, Role>()
            .ForMember(dest => dest.Resources, src => src.Ignore());

        CreateMap<UpdateCommand, Role>()
            .ForMember(dest => dest.Resources, src => src.Ignore())
            .ForMember(dest => dest.Id, src => src.Ignore());
    }
}