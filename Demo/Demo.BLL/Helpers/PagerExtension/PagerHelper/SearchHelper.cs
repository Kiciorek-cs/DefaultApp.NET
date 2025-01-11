using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Demo.BLL.Helpers.PagerExtension.PagerHelper;

public static class SearchHelper
{
    private static readonly string[] formats =
    {
        "d.M", "d/M", "d-M",
        "dd.M", "dd/M", "dd-M",
        "d.MM", "d/MM", "d-MM",
        "dd.MM", "dd/MM", "dd-MM",
        "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyy",
        "d.MM.yyyy", "d/MM/yyyy", "d-MM-yyyy",
        "dd.M.yyyy", "dd/M/yyyy", "dd-M-yyyy",
        "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyyy"
    };

    public static List<Expression<Func<TData, bool>>> CreateSearchListWhereExpressions<TData>(string searchText)
    {
        var listExpression = new List<Expression<Func<TData, bool>>>();
        var getAllProperty = typeof(TData).GetProperties();

        var (_, argumentExpression) = CommandPagerHelper.GetParameter<TData>();

        var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        var likeMethod = typeof(DbFunctionsExtensions).GetMethod("Like",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(DbFunctions), typeof(string), typeof(string) },
            null);

        foreach (var prop in getAllProperty)
        {
            var expression = Expression.Property(argumentExpression, prop);

            var search = Expression.Constant(searchText);

            try
            {
                if (CommandPagerHelper.TypesAllowingToUsingWithContains.Contains(prop.PropertyType) &&
                    !CommandPagerHelper.ColumnsContainingPgrasesNotAllowingToUsingWithContains.Any(x =>
                        prop.Name.Contains(x)))
                {
                    MethodCallExpression containsMethodExp;
                    if (prop.PropertyType != typeof(string) && prop.PropertyType != typeof(DateTime) &&
                        prop.PropertyType != typeof(DateTime?))
                    {
                        containsMethodExp =
                            Expression.Call(Expression.Call(expression, "ToString", null), method!, search);
                    }
                    else if (prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateTime))
                    {
                        containsMethodExp = CreateContainsDateTimeExpression(search, expression, method);
                    }
                    else
                    {
                        search = Expression.Constant($"%{searchText}%");
                        containsMethodExp = Expression.Call(likeMethod,
                            Expression.Property(null, typeof(EF), nameof(EF.Functions)), expression, search);
                        //containsMethodExp = Expression.Call(expression, method!, search);
                    }

                    var predicate = Expression.Lambda<Func<TData, bool>>(containsMethodExp, argumentExpression);

                    listExpression.Add(predicate);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception occurred while transmitting contains.", ex);
            }
        }

        return listExpression;
    }

    private static MethodCallExpression CreateContainsDateTimeExpression(ConstantExpression search,
        MemberExpression expression, MethodInfo method)
    {
        if (DateTime.TryParseExact(search.Value.ToString(), formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dateValue))
        {
            search = Expression.Constant(dateValue.Date.ToString("yyyy-MM-dd"));
        }
        else
        {
            var searchString = search.Value.ToString()?.Replace(".", "-");
            if (searchString != null)
            {
                searchString = searchString.Replace("/", "-");

                search = Expression.Constant(searchString);
            }
        }

        return Expression.Call(Expression.Call(expression, "ToString", null), method, search);
    }
}