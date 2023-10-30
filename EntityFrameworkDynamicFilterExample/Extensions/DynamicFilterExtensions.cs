using System.Linq.Expressions;
using System.Reflection;

namespace EntityFrameworkDynamicFilterExample.Extensions
{
    public static class DynamicFilterExtensions
    {
        public static IEnumerable<T> ApplyFilter<T>(this IQueryable<T> query, QueryCriteria queryCriteria)
        {
            if (queryCriteria.Filters != null)
            {
                foreach (var filter in queryCriteria.Filters)
                {
                    query = query.Where(GetFilterExpressions<T>(filter));
                }
            }

            return query.Skip(queryCriteria.Skip)
                              .Take(queryCriteria.Take)
                              .ToList();
        }

        private static Expression<Func<T, bool>> GetFilterExpressions<T>(Filters filter)
        {
            var paramter = Expression.Parameter(typeof(T));
            var propName = Expression.PropertyOrField(paramter, filter.PropertyName);
            var targetType = propName.Type;

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                targetType = Nullable.GetUnderlyingType(targetType);
            var constExpression = Expression.Constant(Convert.ChangeType(filter.Value, targetType), propName.Type);
            Expression filterExpression;

            switch (filter.Operator)
            {
                case Operator.Equal:
                    filterExpression = Expression.Equal(propName, constExpression);
                    break;

                case Operator.GreaterThanOrEqual:
                    filterExpression = Expression.GreaterThanOrEqual(propName, constExpression);
                    break;

                case Operator.LessThanOrEqual:
                    filterExpression = Expression.LessThanOrEqual(propName, constExpression);
                    break;

                case Operator.GreaterThan:
                    filterExpression = Expression.GreaterThan(propName, constExpression);
                    break;

                case Operator.LessThan:
                    filterExpression = Expression.LessThan(propName, constExpression);
                    break;

                case Operator.NotEqual:
                    filterExpression = Expression.NotEqual(propName, constExpression);
                    break;

                case Operator.Contains:
                    if (filter.Value.GetType() != typeof(string))
                        throw new InvalidFilterCriteriaException();
                    var containsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
                    filterExpression = Expression.Call(propName, containsMethodInfo, constExpression);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return Expression.Lambda<Func<T, bool>>(filterExpression, paramter);
        }
    }
}