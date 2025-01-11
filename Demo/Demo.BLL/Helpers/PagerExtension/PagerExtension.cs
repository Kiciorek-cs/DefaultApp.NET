using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.PagerExtension.PagerHelper;
using Demo.Domain.Enums;
using Demo.Domain.Models.HelperModels.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Demo.BLL.Helpers.PagerExtension;

public static class PagerExtension
{
    public static string PaginationName = "pagination";
    public static string UrlName = "urlHost";

    public static async Task<PaginationModelDto<TData>> PaginationAsync<TData>(
        this IQueryable<TData> query,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
        where TData : class
    {
        return await Pagination(query, httpContextAccessor, cancellationToken);
    }

    public static async Task<IList<TData>> PaginationWithoutModelAsync<TData>(
        this IQueryable<TData> query,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
        where TData : class
    {
        var paged = await Pagination(query, httpContextAccessor, cancellationToken);
        return paged.Data;
    }

    private static async Task<PaginationModelDto<TData>> Pagination<TData>(
        IQueryable<TData> query,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
        where TData : class
    {
        var queryPagination = new UrlQueryPagination();
        queryPagination = httpContextAccessor.ReadDataFromHttpContext(PaginationName, queryPagination);

        try
        {
            var paged = new PaginationModelDto<TData>();

            if (queryPagination.IndexItem is null)
                paged.CurrentPage = queryPagination.Page <= 0 ? 1 : queryPagination.Page;
            else paged.CurrentIndex = queryPagination.IndexItem;

            query = SetWhereExpression(query, paged, queryPagination.WhereColumns);
            if (!string.IsNullOrEmpty(queryPagination.SearchText))
                query = SetSearchExpression(query, queryPagination.SearchText);
            query = SetOrderInQuery(query, queryPagination);

            var itemCount = query.Count();

            paged.TotalItems = itemCount;
            paged.PageSize = queryPagination.PageSize ?? (itemCount > 0 ? itemCount : 1);
            if (paged.TotalItems == 0 || paged.PageSize == 0)
                paged.TotalPages = 0;
            else
                paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (decimal)paged.PageSize);

            var startRow = (queryPagination.Page - 1) * paged.PageSize ?? 1;

            var urlLink = httpContextAccessor.ReadDataFromHttpContext<string>(UrlName);

            var (nextQuery, previousQuery) = LinkGeneratorHelper.GenerateUrl(paged, queryPagination.SearchText,
                queryPagination.IndexItem, queryPagination.OrderColumns, queryPagination.WhereColumns);
            paged.NextPage = !string.IsNullOrEmpty(nextQuery) ? $"{urlLink}{nextQuery}" : null;
            paged.NextQueryPage = !string.IsNullOrEmpty(nextQuery) ? nextQuery : null;
            paged.PreviousPage = !string.IsNullOrEmpty(previousQuery) ? $"{urlLink}{previousQuery}" : null;
            paged.PreviousQueryPage = !string.IsNullOrEmpty(previousQuery) ? previousQuery : null;

            paged.Data = await query
                .AsNoTracking()
                .Skip(startRow)
                .Take(paged.PageSize)
                .AsQueryable()
                .ToListAsync(cancellationToken);

            return paged;
        }
        catch (Exception ex)
        {
            Log.Error("Exception occurred while pagination.", ex);
            return new PaginationModelDto<TData>();
        }
    }

    private static IQueryable<TData> SetOrderInQuery<TData>(IQueryable<TData> query, UrlQueryPagination queryPagination)
    {
        queryPagination.OrderColumns =
            queryPagination.OrderColumns.GroupBy(x => x.Column).Select(group => group.First()).ToList();

        foreach (var order in queryPagination.OrderColumns.Select((x, i) => new { elemet = x, index = i }))
        {
            if (order.index == 0)
            {
                query = SortHelper.ApplyOrder(query, order.elemet.Column,
                    order.elemet.Type == OrderType.Asc ? MethodOrderType.OrderBy : MethodOrderType.OrderByDescending);
                continue;
            }

            query = SortHelper.ApplyOrder(query, order.elemet.Column,
                order.elemet.Type == OrderType.Asc ? MethodOrderType.ThenBy : MethodOrderType.ThenByDescending);
        }

        return query;
    }

    private static IQueryable<TData> SetWhereExpression<TData>(IQueryable<TData> query,
        PaginationModelDto<TData> pagedModel, List<WhereExpression> whereExpressions)
    {
        var listOfWhereExpression = new List<WhereExpression>();

        if (pagedModel.CurrentIndex != null)
            listOfWhereExpression.Add(
                new WhereExpression
                {
                    Column = "Id",
                    Expression = WhereExpressionType.LessThan,
                    Value = pagedModel.CurrentIndex.ToString()
                }
            );

        listOfWhereExpression.AddRange(whereExpressions.Where(x =>
            !CommandPagerHelper.NameOfIdInColumn.Contains(x.Column)));

        var listExpression = ConditionHelper.CreateListWhereExpressions<TData>(listOfWhereExpression);

        var expressionDictionary = CommandPagerHelper.GroupExpressionListByColumn(listExpression);

        foreach (var element in expressionDictionary)
            if (element.Expression.Count > 1)
                query = BuilderLinqHelper.BuildExpression(query, element.Expression, JoinWhereExpression.Or);
            else
                query = BuilderLinqHelper.BuildExpression(query, element.Expression);

        return query;
    }

    private static IQueryable<TData> SetSearchExpression<TData>(IQueryable<TData> query, string searchText)
    {
        var listExpression = SearchHelper.CreateSearchListWhereExpressions<TData>(searchText);

        return BuilderLinqHelper.BuildExpression(query, listExpression, JoinWhereExpression.Or);
    }

    public static U ReadDataFromHttpContext<U>(this IHttpContextAccessor httpContextAccessor, string name,
        U data = default)
    {
        try
        {
            var paginationData = httpContextAccessor?.HttpContext.Items[name];
            if (paginationData != null)
                data = (U)paginationData;

            return data;
        }
        catch (Exception)
        {
            if (typeof(U) == typeof(UrlQueryPagination))
            {
                var serialize = JsonConvert.SerializeObject(new UrlQueryPagination());
                return JsonConvert.DeserializeObject<U>(serialize);
            }
            else
            {
                var serialize = JsonConvert.SerializeObject(string.Empty);
                return JsonConvert.DeserializeObject<U>(serialize);
            }
        }
    }
}

public class UrlQueryPagination
{
    private int _page { get; set; } = 1;
    private int? _indexItem { get; set; }
    private int? _pageSize { get; set; }
    private string _searchText { get; set; }
    private string _materialSearch { get; set; }
    private List<OrderColumn> _orderColumns { get; set; } = new();
    private List<WhereExpression> _whereColumns { get; set; } = new();

    public int? Page
    {
        get => _page;
        set => _page = value ?? 1;
    }

    public int? IndexItem
    {
        get => _indexItem;
        set => _indexItem = value;
    }

    public int? PageSize
    {
        get => _pageSize;
        set => _pageSize = value;
    }

    public string SearchText
    {
        get => _searchText;
        set => _searchText = value;
    }

    public string MaterialSearch
    {
        get => _materialSearch;
        set => _materialSearch = value;
    }

    public List<OrderColumn> OrderColumns
    {
        get => _orderColumns;
        set => _orderColumns = value ?? new List<OrderColumn>();
    }

    public List<WhereExpression> WhereColumns
    {
        get => _whereColumns;
        set => _whereColumns = value ?? new List<WhereExpression>();
    }
}

public class WhereExpression
{
    public string Column { get; set; }
    public string Value { get; set; }
    public WhereExpressionType Expression { get; set; }
}

public class OrderColumn
{
    public string Column { get; set; }
    public OrderType Type { get; set; }
}

public enum JoinWhereExpression
{
    And,
    Or
}