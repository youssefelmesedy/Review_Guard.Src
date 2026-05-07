using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).IsRequired().HasMaxLength(100);

        builder.Property(a => a.Email).HasMaxLength(255);

        builder.Property(a => a.PasswordHash).IsRequired();

        builder.HasIndex(a => a.Email).IsUnique();
    }
}
