using Demo.Domain.Models.DTOModels.Resource;
using MediatR;

namespace Demo.BLL.Features.Resource.Queries.GetById;

public class GetByIdQuery : IRequest<ResourceDto>
{
    public int Id { get; set; }
}