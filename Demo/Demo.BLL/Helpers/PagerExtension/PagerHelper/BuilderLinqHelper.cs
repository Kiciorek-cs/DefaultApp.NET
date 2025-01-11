using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Demo.BLL.Helpers.PagerExtension.PagerHelper;

public static class BuilderLinqHelper
{
    public static IQueryable<TData> BuildExpression<TData>(IQueryable<TData> query,
        List<Expression<Func<TData, bool>>> listExpression,
        JoinWhereExpression joinWhereExpression = JoinWhereExpression.And)
    {
        if (listExpression.Any())
        {
            var allExpressions = listExpression.ElementAt(0);

            for (var i = 1; i < listExpression.Count; i++)
                switch (joinWhereExpression)
                {
                    case JoinWhereExpression.And:
                        allExpressions = allExpressions.AndExpression(listExpression.ElementAt(i));
                        break;
                    case JoinWhereExpression.Or:
                        allExpressions = allExpressions.OrExpression(listExpression.ElementAt(i));
                        break;
                }

            return query.Where(allExpressions);
        }

        return query;
    }

    private static Expression<Func<TData, bool>> AndExpression<TData>(this Expression<Func<TData, bool>> leftExpression,
        Expression<Func<TData, bool>> rightExpression)
    {
        return Combine(leftExpression, rightExpression, Expression.And);
    }

    private static Expression<Func<TData, bool>> OrExpression<TData>(this Expression<Func<TData, bool>> leftExpression,
        Expression<Func<TData, bool>> rightExpression)
    {
        return Combine(leftExpression, rightExpression, Expression.Or);
    }

    private static Expression<Func<TData, bool>> Combine<TData>(Expression<Func<TData, bool>> leftExpression,
        Expression<Func<TData, bool>> rightExpression, Func<Expression, Expression, BinaryExpression> combineOperator)
    {
        var leftParameter = leftExpression.Parameters[0];
        var rightParameter = rightExpression.Parameters[0];

        var visitor = new ReplaceParameterVisitor(rightParameter, leftParameter);

        var leftBody = leftExpression.Body;
        var rightBody = visitor.Visit(rightExpression.Body);

        return Expression.Lambda<Func<TData, bool>>(combineOperator(leftBody, rightBody), leftParameter);
    }
}

internal class ReplaceParameterVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _newParameter;
    private readonly ParameterExpression _oldParameter;

    public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return ReferenceEquals(node, _oldParameter) ? _newParameter : base.VisitParameter(node);
    }
}