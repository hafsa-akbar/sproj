using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using sproj.Data;

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
                .Annotation("Npgsql:Enum:job_category", "babysitting,cleaning,cooking,driving,gardening,laundry,pet_care,security_guard")
                .Annotation("Npgsql:Enum:job_experience", "beginner,expert,intermediate")
                .Annotation("Npgsql:Enum:job_gender", "couple,female,male")
                .Annotation("Npgsql:Enum:job_type", "one_shot,permanent_hire")
                .Annotation("Npgsql:Enum:locale", "islamabad,lahore")
                .Annotation("Npgsql:Enum:role", "employer,unregistered,worker")
                .Annotation("Npgsql:Enum:user_gender", "female,male");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Role = table.Column<Role>(type: "role", nullable: false),
                    FullName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<UserGender>(type: "user_gender", nullable: false),
                    CnicNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    DrivingLicense = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CoupleUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Users_CoupleUserId",
                        column: x => x.CoupleUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "CnicVerification",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CnicImage = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CnicVerification", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CnicVerification_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmsVerification",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VerificationCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsVerification", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_SmsVerification_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobLocale = table.Column<Locale>(type: "locale", nullable: true),
                    JobCategories = table.Column<List<JobCategory>>(type: "job_category[]", nullable: false),
                    JobTypes = table.Column<List<JobType>>(type: "job_type[]", nullable: false),
                    JobExperiences = table.Column<List<JobExperience>>(type: "job_experience[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerDetails",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_WorkerDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsCoupleJob = table.Column<bool>(type: "boolean", nullable: false),
                    WageRate = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    JobGender = table.Column<JobGender>(type: "job_gender", nullable: false),
                    JobCategory = table.Column<JobCategory>(type: "job_category", nullable: false),
                    JobExperience = table.Column<JobExperience>(type: "job_experience", nullable: false),
                    JobType = table.Column<JobType>(type: "job_type", nullable: false),
                    Locale = table.Column<Locale>(type: "locale", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_Jobs_WorkerDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "WorkerDetails",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermanentJob",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    TrialPeriod = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermanentJob", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_PermanentJob_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_UserId",
                table: "Jobs",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CoupleUserId",
                table: "Users",
                column: "CoupleUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CnicVerification");

            migrationBuilder.DropTable(
                name: "PermanentJob");

            migrationBuilder.DropTable(
                name: "SmsVerification");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "WorkerDetails");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
