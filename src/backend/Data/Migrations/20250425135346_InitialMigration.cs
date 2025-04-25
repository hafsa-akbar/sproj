using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace sproj.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:id_type", "driving_license,cnic")
                .Annotation("Npgsql:Enum:job_category", "cooking,cleaning,driving,laundry,gardening,babysitting,pet_care,security_guard")
                .Annotation("Npgsql:Enum:job_experience", "beginner,intermediate,expert")
                .Annotation("Npgsql:Enum:job_gender", "male,female,couple")
                .Annotation("Npgsql:Enum:job_type", "one_shot,permanent_hire")
                .Annotation("Npgsql:Enum:role", "unregistered,employer,worker")
                .Annotation("Npgsql:Enum:user_gender", "male,female");

            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    wage_rate = table.Column<int>(type: "integer", nullable: false),
                    job_category = table.Column<int>(type: "integer", nullable: false),
                    job_experience = table.Column<int>(type: "integer", nullable: false),
                    job_gender = table.Column<int>(type: "integer", nullable: false),
                    job_type = table.Column<int>(type: "integer", nullable: false),
                    locale = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.job_id);
                });

            migrationBuilder.CreateTable(
                name: "past_jobs",
                columns: table => new
                {
                    past_job_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    job_category = table.Column<int>(type: "integer", nullable: false),
                    job_gender = table.Column<int>(type: "integer", nullable: false),
                    job_type = table.Column<int>(type: "integer", nullable: false),
                    locale = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    employer_phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    comments = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_past_jobs", x => x.past_job_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    phone_number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    couple_user_id = table.Column<int>(type: "integer", nullable: true),
                    full_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    cnic_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    driving_license = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_users_users_couple_user_id",
                        column: x => x.couple_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
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

            migrationBuilder.CreateTable(
                name: "cnic_verification",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    id_image = table.Column<byte[]>(type: "bytea", nullable: false),
                    id_type = table.Column<int>(type: "integer", nullable: false)
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
                    job_categories = table.Column<int[]>(type: "integer[]", nullable: false),
                    job_experiences = table.Column<int[]>(type: "integer[]", nullable: false),
                    job_types = table.Column<int[]>(type: "integer[]", nullable: false),
                    job_locale = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
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
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<double>(type: "double precision", nullable: true),
                    number_of_ratings = table.Column<int>(type: "integer", nullable: false)
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
                name: "job_worker_details",
                columns: table => new
                {
                    jobs_job_id = table.Column<int>(type: "integer", nullable: false),
                    worker_details_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_worker_details", x => new { x.jobs_job_id, x.worker_details_user_id });
                    table.ForeignKey(
                        name: "fk_job_worker_details_jobs_jobs_job_id",
                        column: x => x.jobs_job_id,
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_job_worker_details_worker_details_worker_details_user_id",
                        column: x => x.worker_details_user_id,
                        principalTable: "worker_details",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "past_job_worker_details",
                columns: table => new
                {
                    past_jobs_past_job_id = table.Column<int>(type: "integer", nullable: false),
                    worker_details_user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_past_job_worker_details", x => new { x.past_jobs_past_job_id, x.worker_details_user_id });
                    table.ForeignKey(
                        name: "fk_past_job_worker_details_past_jobs_past_jobs_past_job_id",
                        column: x => x.past_jobs_past_job_id,
                        principalTable: "past_jobs",
                        principalColumn: "past_job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_past_job_worker_details_worker_details_worker_details_user_",
                        column: x => x.worker_details_user_id,
                        principalTable: "worker_details",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_job_worker_details_worker_details_user_id",
                table: "job_worker_details",
                column: "worker_details_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_past_job_worker_details_worker_details_user_id",
                table: "past_job_worker_details",
                column: "worker_details_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_couple_user_id",
                table: "users",
                column: "couple_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cnic_verification");

            migrationBuilder.DropTable(
                name: "job_worker_details");

            migrationBuilder.DropTable(
                name: "past_job_worker_details");

            migrationBuilder.DropTable(
                name: "permanent_job");

            migrationBuilder.DropTable(
                name: "sms_verification");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "past_jobs");

            migrationBuilder.DropTable(
                name: "worker_details");

            migrationBuilder.DropTable(
                name: "jobs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
