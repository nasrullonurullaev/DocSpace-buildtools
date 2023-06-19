using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASC.Migrations.PostgreSql.Migrations.CoreDb
{
    /// <inheritdoc />
    public partial class CoreDbContextUpgrade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenants_tenants",
                schema: "onlyoffice",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mappeddomain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, defaultValueSql: "NULL"),
                    version = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "2"),
                    versionchanged = table.Column<DateTime>(name: "version_changed", type: "timestamp with time zone", nullable: true),
                    language = table.Column<string>(type: "character(10)", fixedLength: true, maxLength: 10, nullable: false, defaultValueSql: "'en-US'"),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "NULL"),
                    trusteddomains = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true, defaultValueSql: "NULL"),
                    trusteddomainsenabled = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    status = table.Column<int>(type: "integer", nullable: false),
                    statuschanged = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creationdatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ownerid = table.Column<Guid>(name: "owner_id", type: "uuid", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    paymentid = table.Column<string>(name: "payment_id", type: "character varying(38)", maxLength: 38, nullable: true, defaultValueSql: "NULL"),
                    industry = table.Column<int>(type: "integer", nullable: false),
                    lastmodified = table.Column<DateTime>(name: "last_modified", type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    spam = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true"),
                    calls = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "true")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants_tenants", x => x.id);
                });

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2,
                column: "features",
                value: "audit,ldap,sso,whitelabel,thirdparty,restore,oauth,contentsearch,total_size:107374182400,file_size:1024,manager:1");

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                column: "features",
                value: "trial,audit,ldap,sso,whitelabel,thirdparty,restore,oauth,total_size:107374182400,file_size:100,manager:1");

            migrationBuilder.InsertData(
                schema: "onlyoffice",
                table: "tenants_tenants",
                columns: new[] { "id", "alias", "creationdatetime", "industry", "last_modified", "name", "owner_id", "status", "statuschanged", "version_changed" },
                values: new object[,]
                {
                    { -1, "settings", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("00000000-0000-0000-0000-000000000000"), 1, null, null },
                    { 1, "localhost", new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317), 0, new DateTime(2022, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Web Office", new Guid("66faa6e4-f133-11ea-b126-00ffeec8b4ef"), 0, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "alias",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "last_modified_tenants_tenants",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "last_modified");

            migrationBuilder.CreateIndex(
                name: "mappeddomain",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "mappeddomain");

            migrationBuilder.CreateIndex(
                name: "version",
                schema: "onlyoffice",
                table: "tenants_tenants",
                column: "version");

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_quotarow_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_quotarow",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_tariff_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_tariff",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_tariffrow_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_tariffrow",
                column: "tenant",
                principalSchema: "onlyoffice",
                principalTable: "tenants_tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenants_quotarow_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_quotarow");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_tariff_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_tariff");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_tariffrow_tenants_tenants_tenant",
                schema: "onlyoffice",
                table: "tenants_tariffrow");

            migrationBuilder.DropTable(
                name: "tenants_tenants",
                schema: "onlyoffice");

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -2,
                column: "features",
                value: "audit,ldap,sso,whitelabel,thirdparty,restore,contentsearch,total_size:107374182400,file_size:1024,manager:1");

            migrationBuilder.UpdateData(
                schema: "onlyoffice",
                table: "tenants_quota",
                keyColumn: "tenant",
                keyValue: -1,
                column: "features",
                value: "trial,audit,ldap,sso,whitelabel,thirdparty,restore,total_size:107374182400,file_size:100,manager:1");
        }
    }
}