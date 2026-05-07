using Review_Guard.Application.Abstractions.Repositories.ReviewReppository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.ReviewReppository;

internal sealed class WriteReviewRepository : GenericWriteRepository<Review>, IWriteReviewRepository
{
    public WriteReviewRepository(AppDbContext context) : base(context)
    {
    }
}
