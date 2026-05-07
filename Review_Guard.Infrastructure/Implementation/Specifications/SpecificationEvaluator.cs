using Review_Guard.Application.Abstractions.Specifications;

namespace Review_Guard.Infrastructure.Implementation.Specifications;

internal static class SpecificationEvaluator<TEntity>
 where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(
        IQueryable<TEntity> query,
        ISpecification<TEntity> specification)
    {
        if (specification.AsNoTracking)
            query = query.AsNoTracking();

        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria);

        query = specification.Includes.Aggregate(
            query,
            (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);

        if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);

        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        return query;
    }
}
