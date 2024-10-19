using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace sproj.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aspnet_roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(type: "text", nullable: true),
                    securitystamp = table.Column<string>(type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true),
                    phonenumber = table.Column<string>(type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_roleclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<string>(type: "text", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_roleclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnet_roleclaims_aspnet_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "aspnet_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_userclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<string>(type: "text", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_userclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnet_userclaims_aspnet_users_userid",
                        column: x => x.userid,
                        principalTable: "aspnet_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_userlogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    providerkey = table.Column<string>(type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_userlogins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_aspnet_userlogins_aspnet_users_userid",
                        column: x => x.userid,
                        principalTable: "aspnet_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_userroles",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    roleid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_userroles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_aspnet_userroles_aspnet_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "aspnet_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_aspnet_userroles_aspnet_users_userid",
                        column: x => x.userid,
                        principalTable: "aspnet_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnet_usertokens",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnet_usertokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_aspnet_usertokens_aspnet_users_userid",
                        column: x => x.userid,
                        principalTable: "aspnet_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_roleclaims_roleid",
                table: "aspnet_roleclaims",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "aspnet_roles",
                column: "normalizedname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_userclaims_userid",
                table: "aspnet_userclaims",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_userlogins_userid",
                table: "aspnet_userlogins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnet_userroles_roleid",
                table: "aspnet_userroles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "aspnet_users",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "aspnet_users",
                column: "normalizedusername",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aspnet_roleclaims");

            migrationBuilder.DropTable(
                name: "aspnet_userclaims");

            migrationBuilder.DropTable(
                name: "aspnet_userlogins");

            migrationBuilder.DropTable(
                name: "aspnet_userroles");

            migrationBuilder.DropTable(
                name: "aspnet_usertokens");

            migrationBuilder.DropTable(
                name: "aspnet_roles");

            migrationBuilder.DropTable(
                name: "aspnet_users");
        }
    }
}
