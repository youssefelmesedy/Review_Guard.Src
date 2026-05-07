using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Events;

/// <summary>
/// Raised after a review is submitted.
/// Downstream handlers can trigger notifications, score updates, etc.
/// </summary>
public sealed class ReviewSubmittedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid ReviewId { get; init; }
    public Guid UserId { get; init; }
    public Guid BusinessId { get; init; }
    public ReviewStatus Status { get; init; }
    public decimal RiskScore { get; init; }
    public UserLevel UserLevel { get; init; }
}
