using Review_Guard.Domain.Common;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Domain.Events;

public sealed class RewardAddedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid UserId { get; }

    public UserReward Reward { get; }

    public RewardAddedEvent(Guid userId, UserReward reward)
    {
        UserId = userId;
        Reward = reward;
    }
}