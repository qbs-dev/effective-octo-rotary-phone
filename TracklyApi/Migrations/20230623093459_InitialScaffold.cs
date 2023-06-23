using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TracklyApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialScaffold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    password = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: true),
                    first_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    last_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_access_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_confirmations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user = table.Column<int>(type: "integer", nullable: false),
                    confirmation_type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_confirmations", x => x.id);
                    table.ForeignKey(
                        name: "FK_email_confirmations_users",
                        column: x => x.user,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "managed_urls",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    new_path = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    target_url = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    total_clicks = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_managed_urls", x => x.id);
                    table.ForeignKey(
                        name: "FK_managed_urls_users",
                        column: x => x.user,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_sessions",
                columns: table => new
                {
                    refresh_token = table.Column<Guid>(type: "uuid", nullable: false),
                    user = table.Column<int>(type: "integer", nullable: false),
                    fingerprint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_sessions", x => x.refresh_token);
                    table.ForeignKey(
                        name: "FK_refresh_sessions_users",
                        column: x => x.user,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "url_visits",
                columns: table => new
                {
                    url = table.Column<long>(type: "bigint", nullable: false),
                    visit_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    browser_fingerprint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    redirect_result = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_url_visits", x => new { x.url, x.visit_timestamp });
                    table.ForeignKey(
                        name: "FK_url_visits_managed_urls",
                        column: x => x.url,
                        principalTable: "managed_urls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_email_confirmations_user",
                table: "email_confirmations",
                column: "user");

            migrationBuilder.CreateIndex(
                name: "ix_managed_urls_new_path",
                table: "managed_urls",
                column: "new_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_managed_urls_user",
                table: "managed_urls",
                column: "user");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_sessions_refresh_token",
                table: "refresh_sessions",
                column: "refresh_token");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_sessions_user",
                table: "refresh_sessions",
                column: "user");

            migrationBuilder.CreateIndex(
                name: "ix_url_actions_url",
                table: "url_actions",
                column: "url");

            migrationBuilder.CreateIndex(
                name: "ix_url_visits_country_code",
                table: "url_visits",
                column: "country_code");

            migrationBuilder.CreateIndex(
                name: "ix_url_visits_url",
                table: "url_visits",
                column: "url");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_confirmations");

            migrationBuilder.DropTable(
                name: "refresh_sessions");

            migrationBuilder.DropTable(
                name: "url_actions");

            migrationBuilder.DropTable(
                name: "url_visits");

            migrationBuilder.DropTable(
                name: "managed_urls");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
