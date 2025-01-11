using AutoMapper;
using Demo.BLL.Features.User.Commands.Create;
using Demo.BLL.Features.User.Commands.CreateByUser;
using Demo.BLL.Features.User.Commands.Update;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Models.DTOModels.User;

namespace Demo.BLL.AutoMapper.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.LoginName, src => src.MapFrom(x => x.Login.LoginName))
            .ForMember(dest => dest.EmailAddress, src => src.MapFrom(x => x.Login.EmailAddress))
            .ForMember(dest => dest.TokenGenerationTime, src => src.MapFrom(x => x.Login.TokenGenerationTime))
            .ForMember(dest => dest.EmailValidationStatus, src => src.MapFrom(x => x.Login.EmailValidationStatus))
            .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Login.Status));

        CreateMap<CreateCommand, User>()
            .ForMember(dest => dest.FirstNameUnified, src => src.MapFrom(x => x.FirstName.ToUpper()))
            .ForMember(dest => dest.LastNameUnified, src => src.MapFrom(x => x.LastName.ToUpper()));

        CreateMap<CreateByUserCommand, User>()
            .ForMember(dest => dest.FirstNameUnified, src => src.MapFrom(x => x.FirstName.ToUpper()))
            .ForMember(dest => dest.LastNameUnified, src => src.MapFrom(x => x.LastName.ToUpper()));


        CreateMap<UpdateCommand, User>()
            .ForMember(dest => dest.Id, src => src.Ignore())
            .ForMember(dest => dest.FirstNameUnified, src => src.MapFrom(x => x.FirstName.ToUpper()))
            .ForMember(dest => dest.LastNameUnified, src => src.MapFrom(x => x.LastName.ToUpper()));
    }
}