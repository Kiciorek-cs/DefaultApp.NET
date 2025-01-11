using Demo.Domain.Models.DTOModels.User;
using MediatR;

namespace Demo.BLL.Features.User.Queries.GetOwnAccount;

public class GetOwnAccountQuery : IRequest<UserDto>
{
}