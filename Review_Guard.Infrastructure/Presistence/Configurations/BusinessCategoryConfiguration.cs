using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class BusinessCategoryConfiguration : IEntityTypeConfiguration<BusinessCategory>
{
    public void Configure(EntityTypeBuilder<BusinessCategory> builder)
    {
        builder.ToTable("BusinessCategories");

        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bc => bc.Status).HasConversion<string>()
            .IsRequired();

        builder.HasMany(bc => bc.Businesses)
            .WithOne(b => b.BusinessCategory)
            .HasForeignKey(b => b.BusinessCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
