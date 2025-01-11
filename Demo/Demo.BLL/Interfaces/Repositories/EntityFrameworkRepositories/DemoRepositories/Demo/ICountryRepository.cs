using System.Threading;
using System.Threading.Tasks;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Models.DTOModels.Country;
using Demo.Domain.Models.HelperModels.Pagination;

namespace Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;

public interface ICountryRepository : IRepository<Country>
{
    Task<PaginationModelDto<CountryPaginationDto>> GetWithPagination(CancellationToken cancellationToken);
}