using System.Linq.Expressions;

namespace Review_Guard.Application.Abstractions.Specifications;

public abstract class BaseSpecification<TEntity> : ISpecification<TEntity>
 where TEntity : class
{
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }

    public List<Expression<Func<TEntity, object>>> Includes { get; } = new();

    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

    public int? Skip { get; private set; }

    public int? Take { get; private set; }

    public bool AsNoTracking { get; private set; } = true;

    protected void AddCriteria(Expression<Func<TEntity, bool>> criteria)
        => Criteria = criteria;

    protected void AddInclude(Expression<Func<TEntity, object>> include)
        => Includes.Add(include);

    protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy)
        => OrderBy = orderBy;

    protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDescending)
        => OrderByDescending = orderByDescending;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    protected void EnableTracking()
        => AsNoTracking = false;
}
