using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Events;

/// <summary>
/// Raised when a user's TrustScore changes enough to cross a UserLevel boundary.
/// Useful for triggering UI notifications: "You've been promoted to Trusted!"
/// </summary>
public sealed class UserTrustScoreChangedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid UserId { get; init; }
    public decimal OldScore { get; init; }
    public decimal NewScore { get; init; }
    public UserLevel OldLevel { get; init; }
    public UserLevel NewLevel { get; init; }
    public bool LevelChanged => OldLevel != NewLevel;
}