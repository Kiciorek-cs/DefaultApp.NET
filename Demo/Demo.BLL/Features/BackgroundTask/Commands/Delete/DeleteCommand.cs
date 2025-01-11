using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Commands.Delete;

public class DeleteCommand : IRequest<IResponse>
{
    public string Text { get; set; }
}