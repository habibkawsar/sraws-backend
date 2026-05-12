using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SponsorshipApproval.Api.Infrastructure.Data;

#nullable disable

namespace SponsorshipApproval.Api.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11").HasAnnotation("Relational:MaxIdentifierLength", 63);
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.Role", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
                b.Property<string>("Name").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
                b.HasKey("Id");
                b.HasIndex("Name").IsUnique();
                b.ToTable("Roles");
                b.HasData(new { Id = 1, Name = "Requestor" }, new { Id = 2, Name = "Manager" }, new { Id = 3, Name = "FinanceAdmin" }, new { Id = 4, Name = "SystemAdmin" });
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.SponsorshipType", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
                b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");
                b.Property<string>("Description").HasMaxLength(500).HasColumnType("character varying(500)");
                b.Property<bool>("IsActive").HasColumnType("boolean");
                b.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
                b.HasKey("Id");
                b.HasIndex("Name").IsUnique();
                b.ToTable("SponsorshipTypes");
                b.HasData(
                    new { Id = 1, Name = "Conference", Description = "Industry conference sponsorship", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new { Id = 2, Name = "Community Event", Description = "Community or CSR event sponsorship", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new { Id = 3, Name = "Trade Show", Description = "Expo, trade show, or partner showcase", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.User", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
                b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");
                b.Property<string>("Department").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
                b.Property<string>("Email").IsRequired().HasMaxLength(256).HasColumnType("character varying(256)");
                b.Property<string>("FullName").IsRequired().HasMaxLength(160).HasColumnType("character varying(160)");
                b.Property<bool>("IsActive").HasColumnType("boolean");
                b.Property<string>("PasswordHash").IsRequired().HasMaxLength(256).HasColumnType("character varying(256)");
                b.Property<int>("RoleId").HasColumnType("integer");
                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.HasIndex("RoleId");
                b.ToTable("Users");
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.SponsorshipRequest", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
                b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");
                b.Property<string>("Department").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
                b.Property<DateTime>("EventDate").HasColumnType("timestamp with time zone");
                b.Property<string>("EventOrOrganisationName").IsRequired().HasMaxLength(180).HasColumnType("character varying(180)");
                b.Property<string>("ExpectedBusinessBenefit").IsRequired().HasMaxLength(2000).HasColumnType("character varying(2000)");
                b.Property<string>("PurposeJustification").IsRequired().HasMaxLength(2000).HasColumnType("character varying(2000)");
                b.Property<string>("Remarks").HasMaxLength(1000).HasColumnType("character varying(1000)");
                b.Property<decimal>("RequestedAmount").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
                b.Property<string>("RequestTitle").IsRequired().HasMaxLength(180).HasColumnType("character varying(180)");
                b.Property<string>("RequestorName").IsRequired().HasMaxLength(160).HasColumnType("character varying(160)");
                b.Property<int>("RequestorId").HasColumnType("integer");
                b.Property<int>("SponsorshipTypeId").HasColumnType("integer");
                b.Property<string>("Status").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
                b.Property<DateTime>("UpdatedAt").HasColumnType("timestamp with time zone");
                b.HasKey("Id");
                b.HasIndex("RequestorId");
                b.HasIndex("SponsorshipTypeId");
                b.HasIndex("Status");
                b.ToTable("SponsorshipRequests");
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.ApprovalHistory", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer").HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
                b.Property<string>("Action").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
                b.Property<DateTime>("ActionAt").HasColumnType("timestamp with time zone");
                b.Property<int>("ActionByUserId").HasColumnType("integer");
                b.Property<string>("FromStatus").HasMaxLength(64).HasColumnType("character varying(64)");
                b.Property<string>("Remarks").HasMaxLength(1000).HasColumnType("character varying(1000)");
                b.Property<int>("SponsorshipRequestId").HasColumnType("integer");
                b.Property<string>("ToStatus").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
                b.HasKey("Id");
                b.HasIndex("ActionByUserId");
                b.HasIndex("SponsorshipRequestId", "ActionAt");
                b.ToTable("ApprovalHistories");
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.User", b =>
            {
                b.HasOne("SponsorshipApproval.Api.Domain.Entities.Role", "Role").WithMany("Users").HasForeignKey("RoleId").OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.Navigation("Role");
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.SponsorshipRequest", b =>
            {
                b.HasOne("SponsorshipApproval.Api.Domain.Entities.User", "Requestor").WithMany("SponsorshipRequests").HasForeignKey("RequestorId").OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.HasOne("SponsorshipApproval.Api.Domain.Entities.SponsorshipType", "SponsorshipType").WithMany("SponsorshipRequests").HasForeignKey("SponsorshipTypeId").OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.Navigation("Requestor");
                b.Navigation("SponsorshipType");
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.ApprovalHistory", b =>
            {
                b.HasOne("SponsorshipApproval.Api.Domain.Entities.User", "ActionByUser").WithMany("ApprovalHistories").HasForeignKey("ActionByUserId").OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.HasOne("SponsorshipApproval.Api.Domain.Entities.SponsorshipRequest", "SponsorshipRequest").WithMany("ApprovalHistories").HasForeignKey("SponsorshipRequestId").OnDelete(DeleteBehavior.Cascade).IsRequired();
                b.Navigation("ActionByUser");
                b.Navigation("SponsorshipRequest");
            });

            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.Role", b => b.Navigation("Users"));
            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.SponsorshipType", b => b.Navigation("SponsorshipRequests"));
            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.User", b =>
            {
                b.Navigation("ApprovalHistories");
                b.Navigation("SponsorshipRequests");
            });
            modelBuilder.Entity("SponsorshipApproval.Api.Domain.Entities.SponsorshipRequest", b => b.Navigation("ApprovalHistories"));
        }
    }
}
