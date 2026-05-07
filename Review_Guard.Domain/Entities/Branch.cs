using Review_Guard.Domain.Common;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class Branch : BaseEntity
{
    private Branch() { }

    public string Address { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;

    public Guid BusinessId { get; private set; }
    public Business Business { get; private set; } = default!;

    public Guid ManagerId { get; private set; }
    public User Manager { get; private set; } = default!;

    // Navigation 
    private readonly List<Review> _reviews = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();


    // ── Rating Fields ─────────────────────────
    public decimal SimpleAverageRating { get; private set; }
    public decimal WeightedAverageRating { get; private set; }
    public int TotalReviews { get; private set; }
    public int ApprovedReviewCount { get; private set; }
    public int PendingReviewCount { get; private set; }

    // ── Factory ───────────────────────────────
    public static Branch Create(Guid businessId, string address, string city, string country, string phone, Guid managerId)
    {
        if (string.IsNullOrWhiteSpace(address)) throw new DomainException("Address is required.");
        if (string.IsNullOrWhiteSpace(phone)) throw new DomainException("Phone number is required.");

        return new Branch
        {
            BusinessId = businessId,
            Address = address.Trim(),
            City = city.Trim(),
            Country = country.Trim(),
            PhoneNumber = phone.Trim(),
            ManagerId = managerId
        };
    }


    // ── Manager Actions ───────────────────────
    public void ChangeManager(Guid currentUserId, Guid ownerId, Guid newManagerId)
    {
        if (currentUserId != ownerId) throw new DomainException("Only owner can change manager.");
        ManagerId = newManagerId;
        SetUpdatedAt();
    }

    // ── Weighted Rating Recalculation ─────────────────────────────────────────
    public void RecalculateRatings(
     IEnumerable<(int Rating, decimal TrustWeight)> approvedReviews,
     int pendingCount)
    {
        var list = approvedReviews.ToList();

        ApprovedReviewCount = list.Count;
        TotalReviews = ApprovedReviewCount + pendingCount;

        if (!list.Any())
        {
            SimpleAverageRating = 0;
            WeightedAverageRating = 0;
            SetUpdatedAt();
            return;
        }

        SimpleAverageRating = Math.Round(list.Average(r => (decimal)r.Rating), 2);

        var totalWeight = list.Sum(r => r.TrustWeight);

        WeightedAverageRating = totalWeight == 0
            ? SimpleAverageRating
            : Math.Round(list.Sum(r => r.Rating * r.TrustWeight) / totalWeight, 2);

        SetUpdatedAt();
    }

    public void IncrementPendingReviews() { PendingReviewCount++; SetUpdatedAt(); }
    public void DecrementPendingReviews()
    {
        if (PendingReviewCount > 0) PendingReviewCount--;
        SetUpdatedAt();
    }
}