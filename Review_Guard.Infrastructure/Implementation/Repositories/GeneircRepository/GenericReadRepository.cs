using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Infrastructure.Implementation.Specifications;

namespace Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

internal class GenericReadRepository<TEntity> : IGenericReadRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext _appDbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericReadRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        _dbSet = _appDbContext.Set<TEntity>();
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);
        return await query.AnyAsync(ct);
    }

    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        return predicate is null
            ? await _dbSet.AsNoTracking().CountAsync(ct)
            : await _dbSet.AsNoTracking().CountAsync(predicate, ct);
    }

    public async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);
        return await query.CountAsync(ct);
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(ct);
    }

    public async Task<TEntity?> FindFirstAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, ct);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(
            new object[] { id },
            cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> WhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TResult>> ProjectAsync<TResult>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);

        return await query
            .Select(selector)
            .ToListAsync(ct);
    }

    private IQueryable<TEntity> ApplySpecification(
        ISpecification<TEntity> specification)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(
            _dbSet.AsQueryable(),
            specification);
    }
}