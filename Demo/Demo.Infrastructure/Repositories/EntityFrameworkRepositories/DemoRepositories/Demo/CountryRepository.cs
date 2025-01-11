using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.PagerExtension;
using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Models.DTOModels.Country;
using Demo.Domain.Models.HelperModels.Pagination;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Demo;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;

public class CountryRepository : Repository<DemoContext, Country>, ICountryRepository
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CountryRepository(DemoContext context, IHttpContextAccessor httpContextAccessor) : base(context)
    {
        _httpContextAccessor = httpContextAccessor;

        QueryPagination =
            httpContextAccessor.ReadDataFromHttpContext<UrlQueryPagination>(PagerExtension.PaginationName);
    }

    private UrlQueryPagination QueryPagination { get; set; }

    public async Task<PaginationModelDto<CountryPaginationDto>> GetWithPagination(CancellationToken cancellationToken)
    {
        return await _dbContext.Countries
            .AsNoTracking()
            .Select(x =>
                new CountryPaginationDto
                {
                    Id = x.Id,
                    CallingCode = x.CallingCode,
                    EnglishName = x.EnglishName,
                    IsoCode = x.IsoCode,
                    PolishName = x.PolishName,
                    RegexPhone = x.RegexPhone
                })
            .OrderByDescending(x => x.Id)
            .PaginationAsync(_httpContextAccessor, cancellationToken);
    }
}