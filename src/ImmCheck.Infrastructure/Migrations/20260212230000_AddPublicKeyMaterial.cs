using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImmCheck.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicKeyMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PublicKeyMaterial",
                table: "SigningKeys",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicKeyMaterial",
                table: "SigningKeys");
        }
    }
}
