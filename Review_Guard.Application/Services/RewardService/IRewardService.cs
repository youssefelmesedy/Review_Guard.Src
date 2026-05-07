namespace Review_Guard.Application.Services.RewardService;

public interface IRewardService
{
    Task GrantRewardsAsync(Guid userId, CancellationToken cancellationToken = default);
}
