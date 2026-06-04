using System.Linq.Expressions;

namespace Infrastructure.Extensions;

public static class QueryableExtensions
{
    // Aplica el filtro solo cuando la condicion se cumple, util para queries dinamicas.
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
        => condition ? query.Where(predicate) : query;
}
