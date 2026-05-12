using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SponsorshipApproval.Api.Infrastructure.Data;

#nullable disable

namespace SponsorshipApproval.Api.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("202605110001_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Roles", x => x.Id));

            migrationBuilder.CreateTable(
                name: "SponsorshipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_SponsorshipTypes", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Department = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey("FK_Users_Roles_RoleId", x => x.RoleId, "Roles", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SponsorshipRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestTitle = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    RequestorName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Department = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SponsorshipTypeId = table.Column<int>(type: "integer", nullable: false),
                    EventOrOrganisationName = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PurposeJustification = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ExpectedBusinessBenefit = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RequestorId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorshipRequests", x => x.Id);
                    table.ForeignKey("FK_SponsorshipRequests_SponsorshipTypes_SponsorshipTypeId", x => x.SponsorshipTypeId, "SponsorshipTypes", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_SponsorshipRequests_Users_RequestorId", x => x.RequestorId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorshipRequestId = table.Column<int>(type: "integer", nullable: false),
                    FromStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ToStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ActionByUserId = table.Column<int>(type: "integer", nullable: false),
                    ActionAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistories", x => x.Id);
                    table.ForeignKey("FK_ApprovalHistories_SponsorshipRequests_SponsorshipRequestId", x => x.SponsorshipRequestId, "SponsorshipRequests", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_ApprovalHistories_Users_ActionByUserId", x => x.ActionByUserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                INSERT INTO "Roles" ("Id", "Name") VALUES
                (1, 'Requestor'),
                (2, 'Manager'),
                (3, 'FinanceAdmin'),
                (4, 'SystemAdmin');
                """);
            migrationBuilder.Sql("""
                INSERT INTO "SponsorshipTypes" ("Id", "Name", "Description", "IsActive", "CreatedAt") VALUES
                (1, 'Conference', 'Industry conference sponsorship', true, TIMESTAMPTZ '2026-01-01 00:00:00+00'),
                (2, 'Community Event', 'Community or CSR event sponsorship', true, TIMESTAMPTZ '2026-01-01 00:00:00+00'),
                (3, 'Trade Show', 'Expo, trade show, or partner showcase', true, TIMESTAMPTZ '2026-01-01 00:00:00+00');
                """);
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Roles\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Roles\"));");
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"SponsorshipTypes\"', 'Id'), (SELECT MAX(\"Id\") FROM \"SponsorshipTypes\"));");

            migrationBuilder.CreateIndex("IX_ApprovalHistories_ActionByUserId", "ApprovalHistories", "ActionByUserId");
            migrationBuilder.CreateIndex("IX_ApprovalHistories_SponsorshipRequestId_ActionAt", "ApprovalHistories", new[] { "SponsorshipRequestId", "ActionAt" });
            migrationBuilder.CreateIndex("IX_Roles_Name", "Roles", "Name", unique: true);
            migrationBuilder.CreateIndex("IX_SponsorshipRequests_RequestorId", "SponsorshipRequests", "RequestorId");
            migrationBuilder.CreateIndex("IX_SponsorshipRequests_SponsorshipTypeId", "SponsorshipRequests", "SponsorshipTypeId");
            migrationBuilder.CreateIndex("IX_SponsorshipRequests_Status", "SponsorshipRequests", "Status");
            migrationBuilder.CreateIndex("IX_SponsorshipTypes_Name", "SponsorshipTypes", "Name", unique: true);
            migrationBuilder.CreateIndex("IX_Users_Email", "Users", "Email", unique: true);
            migrationBuilder.CreateIndex("IX_Users_RoleId", "Users", "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("ApprovalHistories");
            migrationBuilder.DropTable("SponsorshipRequests");
            migrationBuilder.DropTable("SponsorshipTypes");
            migrationBuilder.DropTable("Users");
            migrationBuilder.DropTable("Roles");
        }
    }
}
