using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IzhevskEventRadar.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternalId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "InternalId" },
                values: new object[,]
                {
                    { 1, "verbasong" },
                    { 2, "club204542457" },
                    { 3, "solenoe_nebo" },
                    { 4, "hmelion" },
                    { 5, "club93669411" },
                    { 6, "grigger" },
                    { 7, "laskina.voice" },
                    { 8, "club179780208" },
                    { 9, "sunnyjain" },
                    { 10, "club700270" },
                    { 11, "club39964891" },
                    { 12, "waltzing_dogs" },
                    { 13, "jazz18" },
                    { 14, "bardfest_babushkinadacha" },
                    { 15, "bluescrew" },
                    { 16, "mcilchante" },
                    { 17, "snyafrikantza" },
                    { 18, "club77921354" },
                    { 19, "sasha_afrikanets" },
                    { 20, "favorityluny" },
                    { 21, "izhjazzfest" },
                    { 22, "aes_18" },
                    { 23, "udmurtskayatoska" },
                    { 24, "tyloburdo" },
                    { 25, "kareninband" },
                    { 26, "bereg_rock" },
                    { 27, "jahra_reggae" },
                    { 28, "csdr_izh" },
                    { 29, "panamakanal" },
                    { 30, "club123198303" },
                    { 31, "club228777277" },
                    { 32, "ryabchik721" },
                    { 33, "polina_vanez" },
                    { 34, "club224889358" },
                    { 35, "club229240924" },
                    { 36, "club224378859" },
                    { 37, "irisovopole" },
                    { 38, "club224080404" },
                    { 39, "club179780208" },
                    { 40, "udmfil" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
