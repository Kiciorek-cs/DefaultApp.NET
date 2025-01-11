using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.Domain.Models.HelperModels.Pagination;

namespace Demo.BLL.Helpers.PagerExtension.PagerHelper;

public static class LinkGeneratorHelper
{
    public static (string nextQuery, string previousQuery) GenerateUrl<TData>(PaginationModelDto<TData> paged,
        string searchText, int? indexItem, IReadOnlyCollection<OrderColumn> orderColumns,
        IReadOnlyCollection<WhereExpression> whereColumn)
    {
        var nextPage = new StringBuilder();
        var previousPage = new StringBuilder();

        if (paged.TotalPages > 1 && paged.CurrentPage < paged.TotalPages)
            nextPage.Append($"?Page={paged.CurrentPage + 1}");

        if (paged.CurrentPage > 1)
            previousPage.Append($"?Page={paged.CurrentPage - 1}");

        var queryParameters = GenerateConditionQuery(paged.PageSize, searchText, indexItem, orderColumns, whereColumn);

        if (nextPage.Length > 0)
            nextPage.Append(queryParameters);
        if (previousPage.Length > 0)
            previousPage.Append(queryParameters);

        return (nextQuery: nextPage.ToString(), previousQuery: previousPage.ToString());
    }

    private static StringBuilder GenerateConditionQuery(int? pageSize, string searchText, int? indexItem,
        IReadOnlyCollection<OrderColumn> orderColumns, IReadOnlyCollection<WhereExpression> whereColumn)
    {
        var queryParameters = new StringBuilder();
        if (pageSize != null)
            queryParameters.Append($"&PageSize={pageSize}");

        if (!string.IsNullOrEmpty(searchText))
            queryParameters.Append($"&SearchText={searchText}");

        if (indexItem != null)
            queryParameters.Append($"&IndexItem={indexItem}");

        if (orderColumns.Any())
            foreach (var element in orderColumns.Select((x, i) => new { orderElement = x, index = i }))
            {
                queryParameters.Append($"&OrderColumns[{element.index}].Column={element.orderElement.Column}");
                queryParameters.Append($"&OrderColumns[{element.index}].Type={element.orderElement.Type}");
            }

        if (whereColumn.Any())
        {
            whereColumn = whereColumn.Where(x => !CommandPagerHelper.NameOfIdInColumn.Contains(x.Column)).ToList();
            foreach (var element in whereColumn.Select((x, i) => new { whereElement = x, index = i }))
            {
                queryParameters.Append($"&WhereColumns[{element.index}].Column={element.whereElement.Column}");
                queryParameters.Append($"&WhereColumns[{element.index}].Value={element.whereElement.Value}");
                queryParameters.Append($"&WhereColumns[{element.index}].Expression={element.whereElement.Expression}");
            }
        }

        return queryParameters;
    }
}