using Microsoft.EntityFrameworkCore;
using SponsorshipApproval.Api.Domain.Entities;
using SponsorshipApproval.Api.Domain.Enums;

namespace SponsorshipApproval.Api.Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<SponsorshipType> SponsorshipTypes => Set<SponsorshipType>();
    public DbSet<SponsorshipRequest> SponsorshipRequests => Set<SponsorshipRequest>();
    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).IsRequired().HasMaxLength(64);
            entity.HasData(
                new Role { Id = 1, Name = "Requestor" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "FinanceAdmin" },
                new Role { Id = 4, Name = "SystemAdmin" });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(160);
            entity.Property(x => x.Department).IsRequired().HasMaxLength(120);
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
            entity.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SponsorshipType>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.HasData(
                new SponsorshipType { Id = 1, Name = "Conference", Description = "Industry conference sponsorship", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new SponsorshipType { Id = 2, Name = "Community Event", Description = "Community or CSR event sponsorship", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new SponsorshipType { Id = 3, Name = "Trade Show", Description = "Expo, trade show, or partner showcase", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
        });

        modelBuilder.Entity<SponsorshipRequest>(entity =>
        {
            entity.Property(x => x.RequestTitle).IsRequired().HasMaxLength(180);
            entity.Property(x => x.RequestorName).IsRequired().HasMaxLength(160);
            entity.Property(x => x.Department).IsRequired().HasMaxLength(120);
            entity.Property(x => x.EventOrOrganisationName).IsRequired().HasMaxLength(180);
            entity.Property(x => x.RequestedAmount).HasPrecision(18, 2);
            entity.Property(x => x.PurposeJustification).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.ExpectedBusinessBenefit).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.Remarks).HasMaxLength(1000);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(64);
            entity.HasIndex(x => x.Status);
            entity.HasOne(x => x.Requestor).WithMany(x => x.SponsorshipRequests).HasForeignKey(x => x.RequestorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.SponsorshipType).WithMany(x => x.SponsorshipRequests).HasForeignKey(x => x.SponsorshipTypeId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApprovalHistory>(entity =>
        {
            entity.Property(x => x.FromStatus).HasConversion<string>().HasMaxLength(64);
            entity.Property(x => x.ToStatus).HasConversion<string>().HasMaxLength(64);
            entity.Property(x => x.Action).HasConversion<string>().HasMaxLength(64);
            entity.Property(x => x.Remarks).HasMaxLength(1000);
            entity.HasIndex(x => new { x.SponsorshipRequestId, x.ActionAt });
            entity.HasOne(x => x.SponsorshipRequest).WithMany(x => x.ApprovalHistories).HasForeignKey(x => x.SponsorshipRequestId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.ActionByUser).WithMany(x => x.ApprovalHistories).HasForeignKey(x => x.ActionByUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
