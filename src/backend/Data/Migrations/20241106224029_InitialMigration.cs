using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sproj.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_category",
                columns: table => new
                {
                    job_category_id = table.Column<int>(type: "integer", nullable: false),
                    job_category_description = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_category", x => x.job_category_id);
                });

            migrationBuilder.CreateTable(
                name: "job_experience",
                columns: table => new
                {
                    job_experience_id = table.Column<int>(type: "integer", nullable: false),
                    job_experience_description = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_experience", x => x.job_experience_id);
                });

            migrationBuilder.CreateTable(
                name: "job_type",
                columns: table => new
                {
                    job_type_id = table.Column<int>(type: "integer", nullable: false),
                    job_type_description = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_type", x => x.job_type_id);
                });

            migrationBuilder.CreateTable(
                name: "locale",
                columns: table => new
                {
                    locale_id = table.Column<int>(type: "integer", nullable: false),
                    locale_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locale", x => x.locale_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    role_description = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    cnic_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    driving_license = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_users_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cnic_verification",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    cnic_image = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cnic_verification", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_cnic_verification_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sms_verification",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    verification_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sms_verification", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_sms_verification_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    job_locale = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_preferences", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_preferences_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worker_details",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_worker_details", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_worker_details_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_category_prefs",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    job_category_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_category_prefs", x => new { x.user_id, x.job_category_id });
                    table.ForeignKey(
                        name: "fk_job_category_prefs_job_category_job_category_id",
                        column: x => x.job_category_id,
                        principalTable: "job_category",
                        principalColumn: "job_category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_job_category_prefs_user_preferences_user_id",
                        column: x => x.user_id,
                        principalTable: "user_preferences",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_experience_prefs",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    job_experience_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_experience_prefs", x => new { x.user_id, x.job_experience_id });
                    table.ForeignKey(
                        name: "fk_job_experience_prefs_job_experience_job_experience_id",
                        column: x => x.job_experience_id,
                        principalTable: "job_experience",
                        principalColumn: "job_experience_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_job_experience_prefs_user_preferences_user_id",
                        column: x => x.user_id,
                        principalTable: "user_preferences",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "job_type_prefs",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    job_type_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_type_prefs", x => new { x.user_id, x.job_type_id });
                    table.ForeignKey(
                        name: "fk_job_type_prefs_job_type_job_type_id",
                        column: x => x.job_type_id,
                        principalTable: "job_type",
                        principalColumn: "job_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_job_type_prefs_user_preferences_user_id",
                        column: x => x.user_id,
                        principalTable: "user_preferences",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wage_rate = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    job_category_id = table.Column<int>(type: "integer", nullable: false),
                    job_type_id = table.Column<int>(type: "integer", nullable: false),
                    job_experience_id = table.Column<int>(type: "integer", nullable: false),
                    locale_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.job_id);
                    table.ForeignKey(
                        name: "fk_jobs_job_category_job_category_id",
                        column: x => x.job_category_id,
                        principalTable: "job_category",
                        principalColumn: "job_category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_jobs_job_experience_job_experience_id",
                        column: x => x.job_experience_id,
                        principalTable: "job_experience",
                        principalColumn: "job_experience_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_jobs_job_type_job_type_id",
                        column: x => x.job_type_id,
                        principalTable: "job_type",
                        principalColumn: "job_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_jobs_locale_locale_id",
                        column: x => x.locale_id,
                        principalTable: "locale",
                        principalColumn: "locale_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_jobs_worker_details_user_id",
                        column: x => x.user_id,
                        principalTable: "worker_details",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "permanent_job",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false),
                    trial_period = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permanent_job", x => x.job_id);
                    table.ForeignKey(
                        name: "fk_permanent_job_jobs_job_id",
                        column: x => x.job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "job_category",
                columns: new[] { "job_category_id", "job_category_description" },
                values: new object[,]
                {
                    { 1, "cooking" },
                    { 2, "cleaning" },
                    { 3, "driving" },
                    { 4, "laundry" },
                    { 5, "gardening" },
                    { 6, "babysitting" },
                    { 7, "pet_care" },
                    { 8, "securit_guard" }
                });

            migrationBuilder.InsertData(
                table: "job_experience",
                columns: new[] { "job_experience_id", "job_experience_description" },
                values: new object[,]
                {
                    { 1, "beginner" },
                    { 2, "intermediate" },
                    { 3, "expert" }
                });

            migrationBuilder.InsertData(
                table: "job_type",
                columns: new[] { "job_type_id", "job_type_description" },
                values: new object[,]
                {
                    { 1, "one_shot" },
                    { 2, "permanent_hire" }
                });

            migrationBuilder.InsertData(
                table: "locale",
                columns: new[] { "locale_id", "locale_name" },
                values: new object[,]
                {
                    { 1, "lahore" },
                    { 2, "islamabad" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "role_id", "role_description" },
                values: new object[,]
                {
                    { 1, "unregistered" },
                    { 2, "employer" },
                    { 3, "worker" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_job_category_prefs_job_category_id",
                table: "job_category_prefs",
                column: "job_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_experience_prefs_job_experience_id",
                table: "job_experience_prefs",
                column: "job_experience_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_type_prefs_job_type_id",
                table: "job_type_prefs",
                column: "job_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_job_category_id",
                table: "jobs",
                column: "job_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_job_experience_id",
                table: "jobs",
                column: "job_experience_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_job_type_id",
                table: "jobs",
                column: "job_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_locale_id",
                table: "jobs",
                column: "locale_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_user_id",
                table: "jobs",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cnic_verification");

            migrationBuilder.DropTable(
                name: "job_category_prefs");

            migrationBuilder.DropTable(
                name: "job_experience_prefs");

            migrationBuilder.DropTable(
                name: "job_type_prefs");

            migrationBuilder.DropTable(
                name: "permanent_job");

            migrationBuilder.DropTable(
                name: "sms_verification");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "job_category");

            migrationBuilder.DropTable(
                name: "job_experience");

            migrationBuilder.DropTable(
                name: "job_type");

            migrationBuilder.DropTable(
                name: "locale");

            migrationBuilder.DropTable(
                name: "worker_details");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
