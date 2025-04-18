﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using sproj.Data;

#nullable disable

namespace sproj.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "id_type", new[] { "driving_license", "cnic" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_category", new[] { "cooking", "cleaning", "driving", "laundry", "gardening", "babysitting", "pet_care", "security_guard" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_experience", new[] { "beginner", "intermediate", "expert" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_gender", new[] { "male", "female", "couple" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_type", new[] { "one_shot", "permanent_hire" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "role", new[] { "unregistered", "employer", "worker" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_gender", new[] { "male", "female" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("sproj.Data.CnicVerification", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<byte[]>("IdImage")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("id_image");

                    b.Property<int>("IdType")
                        .HasColumnType("integer")
                        .HasColumnName("id_type");

                    b.HasKey("UserId")
                        .HasName("pk_cnic_verification");

                    b.ToTable("cnic_verification", (string)null);
                });

            modelBuilder.Entity("sproj.Data.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("job_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("JobId"));

                    b.Property<int>("JobCategory")
                        .HasColumnType("integer")
                        .HasColumnName("job_category");

                    b.Property<int>("JobExperience")
                        .HasColumnType("integer")
                        .HasColumnName("job_experience");

                    b.Property<int>("JobGender")
                        .HasColumnType("integer")
                        .HasColumnName("job_gender");

                    b.Property<int>("JobType")
                        .HasColumnType("integer")
                        .HasColumnName("job_type");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("locale");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("WageRate")
                        .HasColumnType("integer")
                        .HasColumnName("wage_rate");

                    b.Property<int>("WorkerDetailsUserId")
                        .HasColumnType("integer")
                        .HasColumnName("worker_details_user_id");

                    b.HasKey("JobId")
                        .HasName("pk_jobs");

                    b.HasIndex("WorkerDetailsUserId")
                        .HasDatabaseName("ix_jobs_worker_details_user_id");

                    b.ToTable("jobs", (string)null);
                });

            modelBuilder.Entity("sproj.Data.PastJob", b =>
                {
                    b.Property<int>("PastJobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("past_job_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PastJobId"));

                    b.Property<string>("Comments")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("comments");

                    b.Property<string>("EmployerPhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("employer_phone_number");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean")
                        .HasColumnName("is_verified");

                    b.Property<int>("JobCategory")
                        .HasColumnType("integer")
                        .HasColumnName("job_category");

                    b.Property<int>("JobGender")
                        .HasColumnType("integer")
                        .HasColumnName("job_gender");

                    b.Property<int>("JobType")
                        .HasColumnType("integer")
                        .HasColumnName("job_type");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("locale");

                    b.Property<int?>("Rating")
                        .HasColumnType("integer")
                        .HasColumnName("rating");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("WorkerDetailsUserId")
                        .HasColumnType("integer")
                        .HasColumnName("worker_details_user_id");

                    b.HasKey("PastJobId")
                        .HasName("pk_past_jobs");

                    b.HasIndex("WorkerDetailsUserId")
                        .HasDatabaseName("ix_past_jobs_worker_details_user_id");

                    b.ToTable("past_jobs", (string)null);
                });

            modelBuilder.Entity("sproj.Data.PermanentJob", b =>
                {
                    b.Property<int>("JobId")
                        .HasColumnType("integer")
                        .HasColumnName("job_id");

                    b.Property<int>("TrialPeriod")
                        .HasColumnType("integer")
                        .HasColumnName("trial_period");

                    b.HasKey("JobId")
                        .HasName("pk_permanent_job");

                    b.ToTable("permanent_job", (string)null);
                });

            modelBuilder.Entity("sproj.Data.SmsVerification", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<string>("VerificationCode")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)")
                        .HasColumnName("verification_code");

                    b.HasKey("UserId")
                        .HasName("pk_sms_verification");

                    b.ToTable("sms_verification", (string)null);
                });

            modelBuilder.Entity("sproj.Data.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("address");

                    b.Property<DateOnly>("Birthdate")
                        .HasColumnType("date")
                        .HasColumnName("birthdate");

                    b.Property<string>("CnicNumber")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("cnic_number");

                    b.Property<int?>("CoupleUserId")
                        .HasColumnType("integer")
                        .HasColumnName("couple_user_id");

                    b.Property<string>("DrivingLicense")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("driving_license");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("full_name");

                    b.Property<int>("Gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("password");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)")
                        .HasColumnName("phone_number");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("role");

                    b.HasKey("UserId")
                        .HasName("pk_users");

                    b.HasIndex("CoupleUserId")
                        .HasDatabaseName("ix_users_couple_user_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("sproj.Data.UserPreferences", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.PrimitiveCollection<int[]>("JobCategories")
                        .IsRequired()
                        .HasColumnType("integer[]")
                        .HasColumnName("job_categories");

                    b.PrimitiveCollection<int[]>("JobExperiences")
                        .IsRequired()
                        .HasColumnType("integer[]")
                        .HasColumnName("job_experiences");

                    b.Property<string>("JobLocale")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("job_locale");

                    b.PrimitiveCollection<int[]>("JobTypes")
                        .IsRequired()
                        .HasColumnType("integer[]")
                        .HasColumnName("job_types");

                    b.HasKey("UserId")
                        .HasName("pk_user_preferences");

                    b.ToTable("user_preferences", (string)null);
                });

            modelBuilder.Entity("sproj.Data.WorkerDetails", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("UserId")
                        .HasName("pk_worker_details");

                    b.ToTable("worker_details", (string)null);
                });

            modelBuilder.Entity("sproj.Data.CnicVerification", b =>
                {
                    b.HasOne("sproj.Data.User", null)
                        .WithOne("CnicVerification")
                        .HasForeignKey("sproj.Data.CnicVerification", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cnic_verification_users_user_id");
                });

            modelBuilder.Entity("sproj.Data.Job", b =>
                {
                    b.HasOne("sproj.Data.WorkerDetails", "WorkerDetails")
                        .WithMany("Jobs")
                        .HasForeignKey("WorkerDetailsUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_jobs_worker_details_worker_details_user_id");

                    b.Navigation("WorkerDetails");
                });

            modelBuilder.Entity("sproj.Data.PastJob", b =>
                {
                    b.HasOne("sproj.Data.WorkerDetails", "WorkerDetails")
                        .WithMany("PastJobs")
                        .HasForeignKey("WorkerDetailsUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_past_jobs_worker_details_worker_details_user_id");

                    b.Navigation("WorkerDetails");
                });

            modelBuilder.Entity("sproj.Data.PermanentJob", b =>
                {
                    b.HasOne("sproj.Data.Job", null)
                        .WithOne("PermanentJobDetails")
                        .HasForeignKey("sproj.Data.PermanentJob", "JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_permanent_job_jobs_job_id");
                });

            modelBuilder.Entity("sproj.Data.SmsVerification", b =>
                {
                    b.HasOne("sproj.Data.User", null)
                        .WithOne("SmsVerification")
                        .HasForeignKey("sproj.Data.SmsVerification", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_sms_verification_users_user_id");
                });

            modelBuilder.Entity("sproj.Data.User", b =>
                {
                    b.HasOne("sproj.Data.User", "Couple")
                        .WithMany()
                        .HasForeignKey("CoupleUserId")
                        .HasConstraintName("fk_users_users_couple_user_id");

                    b.Navigation("Couple");
                });

            modelBuilder.Entity("sproj.Data.UserPreferences", b =>
                {
                    b.HasOne("sproj.Data.User", null)
                        .WithOne("UserPreferences")
                        .HasForeignKey("sproj.Data.UserPreferences", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_preferences_users_user_id");
                });

            modelBuilder.Entity("sproj.Data.WorkerDetails", b =>
                {
                    b.HasOne("sproj.Data.User", "User")
                        .WithOne("WorkerDetails")
                        .HasForeignKey("sproj.Data.WorkerDetails", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_worker_details_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("sproj.Data.Job", b =>
                {
                    b.Navigation("PermanentJobDetails");
                });

            modelBuilder.Entity("sproj.Data.User", b =>
                {
                    b.Navigation("CnicVerification");

                    b.Navigation("SmsVerification");

                    b.Navigation("UserPreferences");

                    b.Navigation("WorkerDetails");
                });

            modelBuilder.Entity("sproj.Data.WorkerDetails", b =>
                {
                    b.Navigation("Jobs");

                    b.Navigation("PastJobs");
                });
#pragma warning restore 612, 618
        }
    }
}
