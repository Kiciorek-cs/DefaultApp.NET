using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Demo.BLL.Helpers.PagerExtension.PagerHelper;

public static class CommandPagerHelper
{
    public static readonly List<Type> TypesAllowingToUsingWithContains = new()
        { typeof(string), typeof(int), typeof(decimal), typeof(DateTime), typeof(DateTime?), typeof(double) };

    public static readonly List<Type> NumberWithPrecisionType = new() { typeof(decimal), typeof(double) };
    public static readonly List<string> ColumnsContainingPgrasesNotAllowingToUsingWithContains = new() { "Id" };
    public static readonly List<string> NameOfIdInColumn = new() { "Id" };

    public static Tuple<Type, ParameterExpression> GetParameter<TData>()
    {
        var type = typeof(TData);

        var argumentExpression = Expression.Parameter(type, "x");

        return new Tuple<Type, ParameterExpression>(type, argumentExpression);
    }

    public static PropertyInfo GetPropertyInfoByName(this Type type, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName)) return null;

        return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }

    public static List<ExpressionDictionary<TData>> GroupExpressionListByColumn<TData>(
        List<Expression<Func<TData, bool>>> listExpression)
    {
        var dictionaryExpression = new List<ExpressionDictionary<TData>>();

        foreach (var element in listExpression)
        {
            var expression = element.Body as BinaryExpression;
            if (expression != null)
            {
                var left = (expression.Left as MemberExpression).Member;

                var name = left.Name.ToLower();

                if (dictionaryExpression.Any(x => x.Key == name && x.NodeType == expression.NodeType))
                    dictionaryExpression.First(x => x.Key == name && x.NodeType == expression.NodeType).Expression
                        .Add(element);
                else
                    dictionaryExpression.Add(new ExpressionDictionary<TData>(name, expression.NodeType,
                        new List<Expression<Func<TData, bool>>> { element }));
            }
        }

        return dictionaryExpression;
    }
}

public class ExpressionDictionary<TData>
{
    public ExpressionDictionary(string key, ExpressionType nodeType, List<Expression<Func<TData, bool>>> expression)
    {
        Key = key;
        NodeType = nodeType;
        Expression = expression;
    }

    public string Key { get; }
    public ExpressionType NodeType { get; }
    public List<Expression<Func<TData, bool>>> Expression { get; set; }
}