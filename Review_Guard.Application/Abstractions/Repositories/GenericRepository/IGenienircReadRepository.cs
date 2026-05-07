using Review_Guard.Application.Abstractions.Specifications;
using System.Linq.Expressions;

namespace Review_Guard.Application.Abstractions.Repositories.GenericRepository;

public interface IGenericReadRepository<TEntity>
    where TEntity : class
{
    // Simple queries
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    // Specification queries
    Task<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default);

    Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);
}
