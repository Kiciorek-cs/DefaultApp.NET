using Demo.Domain.Models.DTOModels.User;
using MediatR;

namespace Demo.BLL.Features.User.Queries.GetById;

public class GetByIdQuery : IRequest<UserDto>
{
    public int Id { get; set; }
}