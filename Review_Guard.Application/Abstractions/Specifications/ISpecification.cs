using System.Linq.Expressions;

namespace Review_Guard.Application.Abstractions.Specifications;

public interface ISpecification<TEntity> where TEntity : class
{
    Expression<Func<TEntity, bool>>? Criteria { get; }

    List<Expression<Func<TEntity, object>>> Includes { get; }

    Expression<Func<TEntity, object>>? OrderBy { get; }

    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    int? Skip { get; }

    int? Take { get; }

    bool AsNoTracking { get; }
}
