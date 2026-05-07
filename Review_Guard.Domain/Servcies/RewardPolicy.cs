using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.Servcies;

public static class RewardPolicy
{
    public static IReadOnlyList<RewardType> GetEligibleRewards(decimal trustScore)
    {
        var rewards = new List<RewardType>();

        if (trustScore >= 50)
            rewards.Add(RewardType.Bronze);

        if (trustScore >= 75)
            rewards.Add(RewardType.Silver);

        if (trustScore >= 90)
            rewards.Add(RewardType.Gold);

        return rewards;
    }
}