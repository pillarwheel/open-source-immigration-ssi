using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImmCheck.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCredentialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssuedCredentials",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IssuerDid = table.Column<string>(type: "TEXT", nullable: false),
                    SubjectDid = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialType = table.Column<string>(type: "TEXT", nullable: false),
                    SerializedCredential = table.Column<string>(type: "TEXT", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatusListIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRevoked = table.Column<bool>(type: "INTEGER", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedCredentials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SigningKeys",
                columns: table => new
                {
                    IssuerDid = table.Column<string>(type: "TEXT", nullable: false),
                    KeyMaterial = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Algorithm = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigningKeys", x => x.IssuerDid);
                });

            migrationBuilder.CreateTable(
                name: "StatusLists",
                columns: table => new
                {
                    IssuerDid = table.Column<string>(type: "TEXT", nullable: false),
                    EncodedList = table.Column<string>(type: "TEXT", nullable: false),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    NextIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusLists", x => x.IssuerDid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuedCredentials");

            migrationBuilder.DropTable(
                name: "SigningKeys");

            migrationBuilder.DropTable(
                name: "StatusLists");
        }
    }
}
