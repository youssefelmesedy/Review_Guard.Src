using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Entities;

public sealed class UserReward
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public RewardType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private UserReward()
    { }

    private UserReward(Guid userId, RewardType type)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }


    public static UserReward Create(Guid userId, RewardType type)
        => new(userId, type);
}
