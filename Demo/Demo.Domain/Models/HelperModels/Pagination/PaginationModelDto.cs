using System.Collections.Generic;

namespace Demo.Domain.Models.HelperModels.Pagination;

public class PaginationModelDto<TData>
{
    private const int MaxPageSize = 128;
    private int _pageSize = 15;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public int? CurrentPage { get; set; }
    public int? CurrentIndex { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string NextPage { get; set; }
    public string NextQueryPage { get; set; }
    public string PreviousPage { get; set; }
    public string PreviousQueryPage { get; set; }
    public IList<TData> Data { get; set; } = new List<TData>();
}