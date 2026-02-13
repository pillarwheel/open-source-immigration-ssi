using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace immcheckserverexample.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ds2019info",
                columns: table => new
                {
                    recnum = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idnumber = table.Column<long>(type: "INTEGER", nullable: false),
                    sponsor = table.Column<string>(type: "TEXT", nullable: true),
                    orgCode = table.Column<string>(type: "TEXT", nullable: true),
                    sevisid = table.Column<string>(type: "TEXT", nullable: true),
                    startProgram = table.Column<string>(type: "TEXT", nullable: true),
                    endProgram = table.Column<string>(type: "TEXT", nullable: true),
                    official = table.Column<string>(type: "TEXT", nullable: true),
                    street1 = table.Column<string>(type: "TEXT", nullable: true),
                    street2 = table.Column<string>(type: "TEXT", nullable: true),
                    city = table.Column<string>(type: "TEXT", nullable: true),
                    state = table.Column<string>(type: "TEXT", nullable: true),
                    postal = table.Column<string>(type: "TEXT", nullable: true),
                    phone = table.Column<string>(type: "TEXT", nullable: true),
                    email = table.Column<string>(type: "TEXT", nullable: true),
                    directBill = table.Column<string>(type: "TEXT", nullable: true),
                    datestamp = table.Column<string>(type: "TEXT", nullable: true),
                    ipfsCID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ds2019info", x => x.recnum);
                });

            migrationBuilder.CreateTable(
                name: "I20Program",
                columns: table => new
                {
                    recnum = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idnumber = table.Column<long>(type: "INTEGER", nullable: false),
                    sevisid = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", nullable: true),
                    eduLevel = table.Column<string>(type: "TEXT", nullable: true),
                    eduComments = table.Column<string>(type: "TEXT", nullable: true),
                    primaryMajor = table.Column<string>(type: "TEXT", nullable: true),
                    secondMajor = table.Column<string>(type: "TEXT", nullable: true),
                    minor = table.Column<string>(type: "TEXT", nullable: true),
                    lengthOfStudy = table.Column<string>(type: "TEXT", nullable: true),
                    prgStartDate = table.Column<string>(type: "TEXT", nullable: true),
                    prgEndDate = table.Column<string>(type: "TEXT", nullable: true),
                    engRequired = table.Column<string>(type: "TEXT", nullable: true),
                    engRequirementsMet = table.Column<string>(type: "TEXT", nullable: true),
                    engNotRequired = table.Column<string>(type: "TEXT", nullable: true),
                    datestamp = table.Column<string>(type: "TEXT", nullable: true),
                    institutionalKey = table.Column<string>(type: "TEXT", nullable: true),
                    issDate = table.Column<string>(type: "TEXT", nullable: true),
                    ipfsCID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_I20Program", x => x.recnum);
                });

            migrationBuilder.CreateTable(
                name: "I94",
                columns: table => new
                {
                    recnum = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idnumber = table.Column<long>(type: "INTEGER", nullable: false),
                    i94num = table.Column<string>(type: "TEXT", nullable: true),
                    i94poe = table.Column<string>(type: "TEXT", nullable: true),
                    i94iss = table.Column<string>(type: "TEXT", nullable: true),
                    i94exp = table.Column<string>(type: "TEXT", nullable: true),
                    i94expds = table.Column<string>(type: "TEXT", nullable: true),
                    datestamp = table.Column<string>(type: "TEXT", nullable: true),
                    ipfsCID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_I94", x => x.recnum);
                });

            migrationBuilder.CreateTable(
                name: "passport",
                columns: table => new
                {
                    recnum = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idnumber = table.Column<long>(type: "INTEGER", nullable: false),
                    givenName = table.Column<string>(type: "TEXT", nullable: true),
                    lastname = table.Column<string>(type: "TEXT", nullable: true),
                    cpass = table.Column<string>(type: "TEXT", nullable: true),
                    passnum = table.Column<string>(type: "TEXT", nullable: true),
                    passiss = table.Column<string>(type: "TEXT", nullable: true),
                    passexp = table.Column<string>(type: "TEXT", nullable: true),
                    datestamp = table.Column<string>(type: "TEXT", nullable: true),
                    ipfsCID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passport", x => x.recnum);
                });

            migrationBuilder.CreateTable(
                name: "sponsoredStudentInformation",
                columns: table => new
                {
                    recnum = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idnumber = table.Column<long>(type: "INTEGER", nullable: false),
                    sponsor = table.Column<string>(type: "TEXT", nullable: true),
                    fundingSponsor = table.Column<string>(type: "TEXT", nullable: true),
                    advisorName = table.Column<string>(type: "TEXT", nullable: true),
                    advisorEmail = table.Column<string>(type: "TEXT", nullable: true),
                    advisorPhone = table.Column<string>(type: "TEXT", nullable: true),
                    isSponsoredStudent = table.Column<string>(type: "TEXT", nullable: true),
                    datestamp = table.Column<string>(type: "TEXT", nullable: true),
                    isFeeExempt = table.Column<string>(type: "TEXT", nullable: true),
                    isUSGovSponsored = table.Column<string>(type: "TEXT", nullable: true),
                    needsFinancialDocs = table.Column<string>(type: "TEXT", nullable: true),
                    financialDocsExpDate = table.Column<string>(type: "TEXT", nullable: true),
                    ipfsCID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsoredStudentInformation", x => x.recnum);
                });

            migrationBuilder.CreateTable(
                name: "visaInfo",
                columns: table => new
                {
                    recnum = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idnumber = table.Column<long>(type: "INTEGER", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: true),
                    visastamp = table.Column<string>(type: "TEXT", nullable: true),
                    stampnum = table.Column<string>(type: "TEXT", nullable: true),
                    controlnumber = table.Column<string>(type: "TEXT", nullable: true),
                    stampplace = table.Column<string>(type: "TEXT", nullable: true),
                    stampent = table.Column<string>(type: "TEXT", nullable: true),
                    stampiss = table.Column<string>(type: "TEXT", nullable: true),
                    stampexp = table.Column<string>(type: "TEXT", nullable: true),
                    datestamp = table.Column<string>(type: "TEXT", nullable: true),
                    ipfsCID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visaInfo", x => x.recnum);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ds2019info");

            migrationBuilder.DropTable(
                name: "I20Program");

            migrationBuilder.DropTable(
                name: "I94");

            migrationBuilder.DropTable(
                name: "passport");

            migrationBuilder.DropTable(
                name: "sponsoredStudentInformation");

            migrationBuilder.DropTable(
                name: "visaInfo");
        }
    }
}
