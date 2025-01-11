using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Commands.Create;

public class CreateCommand : IRequest<IResponse>
{
    public string Text { get; set; }
    public int Delay { get; set; }
}