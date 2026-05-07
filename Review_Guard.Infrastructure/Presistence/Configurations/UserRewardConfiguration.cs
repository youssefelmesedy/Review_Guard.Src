using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class UserRewardConfiguration : IEntityTypeConfiguration<UserReward>
{
    public void Configure(EntityTypeBuilder<UserReward> builder)
    {
        builder.ToTable("UserRewards");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Type })
            .IsUnique();
    }
}
