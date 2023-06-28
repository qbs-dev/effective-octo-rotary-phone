using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TracklyApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUrlActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "url_actions");

            migrationBuilder.DropColumn(
                name: "redirect_result",
                table: "url_visits");

            migrationBuilder.AlterColumn<string>(
                name: "country_code",
                table: "url_visits",
                type: "character varying(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "country_code",
                table: "url_visits",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2);

            migrationBuilder.AddColumn<int>(
                name: "redirect_result",
                table: "url_visits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "url_actions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    url = table.Column<long>(type: "bigint", nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_url_actions", x => x.id);
                    table.ForeignKey(
                        name: "FK_url_actions_managed_urls",
                        column: x => x.url,
                        principalTable: "managed_urls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_url_actions_url",
                table: "url_actions",
                column: "url");
        }
    }
}
