using Demo.Domain.Models.DTOModels.Country;
using MediatR;

namespace Demo.BLL.Features.Country.Queries.GetById;

public class GetByIdQuery : IRequest<CountryDto>
{
    public int Id { get; set; }
}