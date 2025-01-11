using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Commands.DeleteAll;

public class DeleteAllCommand : IRequest<IResponse>
{
}