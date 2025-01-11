using Demo.Domain.Models.DTOModels.BackgroundTask;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Queries.GetList;

public class GetListQuery : IRequest<BackgroundTaskDto>
{
}