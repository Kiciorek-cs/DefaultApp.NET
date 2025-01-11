using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Helpers.Singletons;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.BackgroundTask;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Queries.GetList;

public class
    GetListQueryHandler : IRequestHandler<GetListQuery, BackgroundTaskDto>
{
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly IMapper _mapper;

    public GetListQueryHandler(
        IMapper mapper, IDemoUnitOfWork demoUnitOfWork)
    {
        _mapper = mapper;
        _demoUnitOfWork = demoUnitOfWork;
    }

    public async Task<BackgroundTaskDto> Handle(GetListQuery request,
        CancellationToken cancellationToken)
    {
        var listOfBackgroundTasksFromDatabase =
            await _demoUnitOfWork.BackgroundTaskRepository.GetMultipleNoTrackingAsync(null, cancellationToken);

        var listOfBackgroundTasksFromDatabaseMapped =
            _mapper.Map<IEnumerable<BackgroundTaskInformationDto>>(listOfBackgroundTasksFromDatabase);

        var actionBlocks = await GetAllActionBlock();

        var actionBlockMapped = _mapper.Map<IEnumerable<BackgroundTaskInformationDto>>(actionBlocks);

        var backgroundTaskDto = new BackgroundTaskDto
        {
            DatabaseTasks = listOfBackgroundTasksFromDatabaseMapped.ToList(),
            WorkingTasks = actionBlockMapped.ToList()
        };

        return backgroundTaskDto;
    }

    private async Task<List<ActionBlockModel<Tuple<Guid, CancellationToken, SemaphoreSlim, string>>>> GetAllActionBlock()
    {
        var singletonFtp = ActionBlockSingleton<Tuple<Guid, CancellationToken, SemaphoreSlim, string>>.GetInstance();

        return singletonFtp.GetAllActionBlock();
    }
}