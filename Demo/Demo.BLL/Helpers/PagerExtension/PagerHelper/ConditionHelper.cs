using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Demo.Domain.Enums;
using Serilog;

namespace Demo.BLL.Helpers.PagerExtension.PagerHelper;

public static class ConditionHelper
{
    public static List<Expression<Func<TData, bool>>> CreateListWhereExpressions<TData>(
        List<WhereExpression> expressions)
    {
        var listExpression = new List<Expression<Func<TData, bool>>>();

        var (type, argumentExpression) = CommandPagerHelper.GetParameter<TData>();

        foreach (var item in expressions)
        {
            var property = type.GetPropertyInfoByName(item.Column);

            if (property is null) continue;

            var expression = Expression.Property(argumentExpression, property);

            var propertyType = property.PropertyType;
            try
            {
                if (CommandPagerHelper.NumberWithPrecisionType.Contains(propertyType))
                    if (item.Value.Contains("."))
                    {
                        item.Value = item.Value.Replace(",", string.Empty);
                        item.Value = item.Value.Replace(".", ",");
                        item.Value = item.Value.Replace(" ", string.Empty);
                    }

                var convertedValue = Convert.ChangeType(item.Value, propertyType);

                var constant = Expression.Constant(convertedValue, propertyType);

                var binary = CreateBinaryExpression(expression, constant, item.Expression);

                listExpression.Add((Expression<Func<TData, bool>>)Expression.Lambda(binary, argumentExpression));
            }
            catch (Exception ex)
            {
                Log.Error("Exception occurred while converting value.", ex);
            }
        }

        return listExpression;
    }

    private static Expression CreateBinaryExpression(Expression member, Expression constant,
        WhereExpressionType expressionType)
    {
        return expressionType switch
        {
            WhereExpressionType.Equal => Expression.Equal(member, constant),
            WhereExpressionType.NotEqual => Expression.NotEqual(member, constant),
            WhereExpressionType.GreaterThan => Expression.GreaterThan(member, constant),
            WhereExpressionType.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
            WhereExpressionType.LessThan => Expression.LessThan(member, constant),
            WhereExpressionType.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
            _ => Expression.Equal(member, constant)
        };
    }
}