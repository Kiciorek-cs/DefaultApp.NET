using AutoMapper;
using Demo.BLL.Helpers.Singletons;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Models.DTOModels.BackgroundTask;
using System;
using System.Threading;

namespace Demo.BLL.AutoMapper.Profiles;

public class BackgroundTaskProfile : Profile
{
    public BackgroundTaskProfile()
    {
        CreateMap<BackgroundTask, BackgroundTaskDto>();

        CreateMap<BackgroundTask, BackgroundTaskInformationDto>()
            .ForMember(dest => dest.Key, src => src.MapFrom(x => x.ActionBlockKey));

        CreateMap<ActionBlockModel<Tuple<Guid, CancellationToken, SemaphoreSlim, string>>,
                BackgroundTaskInformationDto>()
            .ForMember(dest => dest.Text, src => src.MapFrom(x => x.Text))
            .ForMember(dest => dest.Key, src => src.MapFrom(x => x.Id));

        CreateMap<ActionBlockModel<Tuple<Guid, CancellationToken, SemaphoreSlim, string>>, BackgroundTaskDto>();

    }
}