using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImmCheck.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Existing document tables: use IF NOT EXISTS to avoid errors on pre-existing DBs ---
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""ds2019info"" (
                    ""recnum"" INTEGER NOT NULL CONSTRAINT ""PK_ds2019info"" PRIMARY KEY AUTOINCREMENT,
                    ""idnumber"" INTEGER NOT NULL,
                    ""sponsor"" TEXT NULL,
                    ""orgCode"" TEXT NULL,
                    ""sevisid"" TEXT NULL,
                    ""startProgram"" TEXT NULL,
                    ""endProgram"" TEXT NULL,
                    ""official"" TEXT NULL,
                    ""street1"" TEXT NULL,
                    ""street2"" TEXT NULL,
                    ""city"" TEXT NULL,
                    ""state"" TEXT NULL,
                    ""postal"" TEXT NULL,
                    ""phone"" TEXT NULL,
                    ""email"" TEXT NULL,
                    ""directBill"" TEXT NULL,
                    ""datestamp"" TEXT NULL,
                    ""ipfsCID"" TEXT NULL
                );");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""I20Program"" (
                    ""recnum"" INTEGER NOT NULL CONSTRAINT ""PK_I20Program"" PRIMARY KEY AUTOINCREMENT,
                    ""idnumber"" INTEGER NOT NULL,
                    ""sevisid"" TEXT NULL,
                    ""status"" TEXT NULL,
                    ""eduLevel"" TEXT NULL,
                    ""eduComments"" TEXT NULL,
                    ""primaryMajor"" TEXT NULL,
                    ""secondMajor"" TEXT NULL,
                    ""minor"" TEXT NULL,
                    ""lengthOfStudy"" TEXT NULL,
                    ""prgStartDate"" TEXT NULL,
                    ""prgEndDate"" TEXT NULL,
                    ""engRequired"" TEXT NULL,
                    ""engRequirementsMet"" TEXT NULL,
                    ""engNotRequired"" TEXT NULL,
                    ""datestamp"" TEXT NULL,
                    ""institutionalKey"" TEXT NULL,
                    ""issDate"" TEXT NULL,
                    ""ipfsCID"" TEXT NULL
                );");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""I94"" (
                    ""recnum"" INTEGER NOT NULL CONSTRAINT ""PK_I94"" PRIMARY KEY AUTOINCREMENT,
                    ""idnumber"" INTEGER NOT NULL,
                    ""i94num"" TEXT NULL,
                    ""i94poe"" TEXT NULL,
                    ""i94iss"" TEXT NULL,
                    ""i94exp"" TEXT NULL,
                    ""i94expds"" TEXT NULL,
                    ""datestamp"" TEXT NULL,
                    ""ipfsCID"" TEXT NULL
                );");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""passport"" (
                    ""recnum"" INTEGER NOT NULL CONSTRAINT ""PK_passport"" PRIMARY KEY AUTOINCREMENT,
                    ""idnumber"" INTEGER NOT NULL,
                    ""givenName"" TEXT NULL,
                    ""lastname"" TEXT NULL,
                    ""cpass"" TEXT NULL,
                    ""passnum"" TEXT NULL,
                    ""passiss"" TEXT NULL,
                    ""passexp"" TEXT NULL,
                    ""datestamp"" TEXT NULL,
                    ""ipfsCID"" TEXT NULL
                );");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""sevisI20Financial"" (
                    ""recnum"" INTEGER NOT NULL CONSTRAINT ""PK_sevisI20Financial"" PRIMARY KEY AUTOINCREMENT,
                    ""idnumber"" INTEGER NOT NULL,
                    ""sevisid"" TEXT NULL,
                    ""academicTerm"" TEXT NULL,
                    ""tuition"" INTEGER NULL,
                    ""livingExpenses"" INTEGER NULL,
                    ""dependentExp"" INTEGER NULL,
                    ""otherExpAmount"" INTEGER NULL,
                    ""otherExpDesc"" TEXT NULL,
                    ""personalFunds"" INTEGER NULL,
                    ""schoolFundsAmount"" INTEGER NULL,
                    ""schoolFundsDesc"" TEXT NULL,
                    ""otherFundsAmount"" INTEGER NULL,
                    ""otherFundsDesc"" TEXT NULL,
                    ""employmentFunds"" INTEGER NULL,
                    ""remarks"" TEXT NULL,
                    ""datestamp"" TEXT NULL,
                    ""ipfsCID"" TEXT NULL
                );");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""sponsoredStudentInformation"" (
                    ""recnum"" INTEGER NOT NULL CONSTRAINT ""PK_sponsoredStudentInformation"" PRIMARY KEY AUTOINCREMENT,
                    ""idnumber"" INTEGER NOT NULL,
                    ""sponsor"" TEXT NULL,
                    ""fundingSponsor"" TEXT NULL,
                    ""advisorName"" TEXT NULL,
                    ""advisorEmail"" TEXT NULL,
                    ""advisorPhone"" TEXT NULL,
                    ""isSponsoredStudent"" TEXT NULL,
                    ""datestamp"" TEXT NULL,
                    ""isFeeExempt"" TEXT NULL,
                    ""isUSGovSponsored"" TEXT NULL,
                    ""needsFinancialDocs"" TEXT NULL,
                    ""financialDocsExpDate"" TEXT NULL,
                    ""ipfsCID"" TEXT NULL
                );");

            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""visaInfo"" (
                    ""recnum"" INTEGER NOT NULL CONSTRAINT ""PK_visaInfo"" PRIMARY KEY AUTOINCREMENT,
                    ""idnumber"" INTEGER NOT NULL,
                    ""status"" TEXT NULL,
                    ""visastamp"" TEXT NULL,
                    ""stampnum"" TEXT NULL,
                    ""controlnumber"" TEXT NULL,
                    ""stampplace"" TEXT NULL,
                    ""stampent"" TEXT NULL,
                    ""stampiss"" TEXT NULL,
                    ""stampexp"" TEXT NULL,
                    ""datestamp"" TEXT NULL,
                    ""ipfsCID"" TEXT NULL
                );");

            // --- New tables: Identity + Institutions ---
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SchoolCode = table.Column<string>(type: "TEXT", nullable: true),
                    Did = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    InstitutionId = table.Column<long>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_InstitutionId",
                table: "AspNetUsers",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AspNetRoleClaims");
            migrationBuilder.DropTable(name: "AspNetUserClaims");
            migrationBuilder.DropTable(name: "AspNetUserLogins");
            migrationBuilder.DropTable(name: "AspNetUserRoles");
            migrationBuilder.DropTable(name: "AspNetUserTokens");
            migrationBuilder.DropTable(name: "AspNetRoles");
            migrationBuilder.DropTable(name: "AspNetUsers");
            migrationBuilder.DropTable(name: "Institutions");
            // Note: document tables are not dropped as they pre-existed this migration
        }
    }
}
