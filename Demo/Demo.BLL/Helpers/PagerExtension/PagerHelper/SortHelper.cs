using System;
using System.Linq;
using System.Linq.Expressions;
using Demo.Domain.Enums;

namespace Demo.BLL.Helpers.PagerExtension.PagerHelper;

public static class SortHelper
{
    public static IQueryable<TData> ApplyOrder<TData>(IQueryable<TData> query, string orderByProperty,
        MethodOrderType methodName)
    {
        var (type, argumentExpression) = CommandPagerHelper.GetParameter<TData>();

        var property = type.GetPropertyInfoByName(orderByProperty);

        if (property is null) return query;

        var expression = Expression.Property(argumentExpression, property);
        var propertyType = property.PropertyType;

        var delegateType = typeof(Func<,>).MakeGenericType(typeof(TData), propertyType);
        var lambda = Expression.Lambda(delegateType, expression, argumentExpression);

        var result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName.ToString()
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TData), propertyType)
            .Invoke(null, new object[] { query, lambda });

        return (IOrderedQueryable<TData>)result;
    }
}