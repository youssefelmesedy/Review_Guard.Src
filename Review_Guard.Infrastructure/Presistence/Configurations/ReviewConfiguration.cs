using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        // ── Key ─────────────────────────────
        builder.HasKey(r => r.Id);

        // ── Properties ──────────────────────
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.OverallRating)
            .HasPrecision(2, 1);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.AdminNote)
            .HasMaxLength(1000);

        // ── Ratings ─────────────────────────
        builder.Property(r => r.FoodRating).IsRequired();
        builder.Property(r => r.ServiceRating).IsRequired();
        builder.Property(r => r.CleanlinessRating).IsRequired();
        builder.Property(r => r.AmbienceRating).IsRequired();
        builder.Property(r => r.ValueRating).IsRequired();

        // ── Relationships ───────────────────

        // Review → User
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Review → Proof
        builder.HasOne(r => r.Proof)
            .WithOne()
            .HasForeignKey<Review>(r => r.ProofId)
            .OnDelete(DeleteBehavior.SetNull);

        // Review → Branch
        builder.HasOne(r => r.Branch)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BranchId)
            .OnDelete(DeleteBehavior.Restrict);


        // ── Indexes (Performance) ─────────
        builder.HasIndex(r => r.BranchId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.Status);
    }
}
