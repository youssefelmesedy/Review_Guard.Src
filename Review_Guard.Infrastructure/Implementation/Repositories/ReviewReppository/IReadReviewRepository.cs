using Review_Guard.Application.Abstractions.Repositories.ReviewReppository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.ReviewReppository;

internal sealed class ReadReviewRepository : GenericReadRepository<Review>, IReadReviewRepository
{
    public ReadReviewRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
