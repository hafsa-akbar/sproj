﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using sproj.Data;

#nullable disable

namespace sproj.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241107003525_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("job_category_prefs", b =>
                {
                    b.Property<int>("user_id")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("job_category_id")
                        .HasColumnType("integer")
                        .HasColumnName("job_category_id");

                    b.HasKey("user_id", "job_category_id")
                        .HasName("pk_job_category_prefs");

                    b.HasIndex("job_category_id")
                        .HasDatabaseName("ix_job_category_prefs_job_category_id");

                    b.ToTable("job_category_prefs", (string)null);
                });

            modelBuilder.Entity("job_experience_prefs", b =>
                {
                    b.Property<int>("user_id")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("job_experience_id")
                        .HasColumnType("integer")
                        .HasColumnName("job_experience_id");

                    b.HasKey("user_id", "job_experience_id")
                        .HasName("pk_job_experience_prefs");

                    b.HasIndex("job_experience_id")
                        .HasDatabaseName("ix_job_experience_prefs_job_experience_id");

                    b.ToTable("job_experience_prefs", (string)null);
                });

            modelBuilder.Entity("job_type_prefs", b =>
                {
                    b.Property<int>("user_id")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("job_type_id")
                        .HasColumnType("integer")
                        .HasColumnName("job_type_id");

                    b.HasKey("user_id", "job_type_id")
                        .HasName("pk_job_type_prefs");

                    b.HasIndex("job_type_id")
                        .HasDatabaseName("ix_job_type_prefs_job_type_id");

                    b.ToTable("job_type_prefs", (string)null);
                });

            modelBuilder.Entity("sproj.Data.CnicVerification", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<byte[]>("CnicImage")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("cnic_image");

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

                    b.Property<int>("JobCategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("job_category_id");

                    b.Property<int>("JobExperienceId")
                        .HasColumnType("integer")
                        .HasColumnName("job_experience_id");

                    b.Property<int>("JobTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("job_type_id");

                    b.Property<int>("LocaleId")
                        .HasColumnType("integer")
                        .HasColumnName("locale_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("WageRate")
                        .HasColumnType("integer")
                        .HasColumnName("wage_rate");

                    b.HasKey("JobId")
                        .HasName("pk_jobs");

                    b.HasIndex("JobCategoryId")
                        .HasDatabaseName("ix_jobs_job_category_id");

                    b.HasIndex("JobExperienceId")
                        .HasDatabaseName("ix_jobs_job_experience_id");

                    b.HasIndex("JobTypeId")
                        .HasDatabaseName("ix_jobs_job_type_id");

                    b.HasIndex("LocaleId")
                        .HasDatabaseName("ix_jobs_locale_id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_jobs_user_id");

                    b.ToTable("jobs", (string)null);
                });

            modelBuilder.Entity("sproj.Data.JobCategory", b =>
                {
                    b.Property<int>("JobCategoryId")
                        .HasColumnType("integer")
                        .HasColumnName("job_category_id");

                    b.Property<string>("JobCategoryDescription")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("job_category_description");

                    b.HasKey("JobCategoryId")
                        .HasName("pk_job_category");

                    b.ToTable("job_category", (string)null);

                    b.HasData(
                        new
                        {
                            JobCategoryId = 1,
                            JobCategoryDescription = "cooking"
                        },
                        new
                        {
                            JobCategoryId = 2,
                            JobCategoryDescription = "cleaning"
                        },
                        new
                        {
                            JobCategoryId = 3,
                            JobCategoryDescription = "driving"
                        },
                        new
                        {
                            JobCategoryId = 4,
                            JobCategoryDescription = "laundry"
                        },
                        new
                        {
                            JobCategoryId = 5,
                            JobCategoryDescription = "gardening"
                        },
                        new
                        {
                            JobCategoryId = 6,
                            JobCategoryDescription = "babysitting"
                        },
                        new
                        {
                            JobCategoryId = 7,
                            JobCategoryDescription = "pet_care"
                        },
                        new
                        {
                            JobCategoryId = 8,
                            JobCategoryDescription = "securit_guard"
                        });
                });

            modelBuilder.Entity("sproj.Data.JobExperience", b =>
                {
                    b.Property<int>("JobExperienceId")
                        .HasColumnType("integer")
                        .HasColumnName("job_experience_id");

                    b.Property<string>("JobExperienceDescription")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("job_experience_description");

                    b.HasKey("JobExperienceId")
                        .HasName("pk_job_experience");

                    b.ToTable("job_experience", (string)null);

                    b.HasData(
                        new
                        {
                            JobExperienceId = 1,
                            JobExperienceDescription = "beginner"
                        },
                        new
                        {
                            JobExperienceId = 2,
                            JobExperienceDescription = "intermediate"
                        },
                        new
                        {
                            JobExperienceId = 3,
                            JobExperienceDescription = "expert"
                        });
                });

            modelBuilder.Entity("sproj.Data.JobType", b =>
                {
                    b.Property<int>("JobTypeId")
                        .HasColumnType("integer")
                        .HasColumnName("job_type_id");

                    b.Property<string>("JobTypeDescription")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("job_type_description");

                    b.HasKey("JobTypeId")
                        .HasName("pk_job_type");

                    b.ToTable("job_type", (string)null);

                    b.HasData(
                        new
                        {
                            JobTypeId = 1,
                            JobTypeDescription = "one_shot"
                        },
                        new
                        {
                            JobTypeId = 2,
                            JobTypeDescription = "permanent_hire"
                        });
                });

            modelBuilder.Entity("sproj.Data.Locale", b =>
                {
                    b.Property<int>("LocaleId")
                        .HasColumnType("integer")
                        .HasColumnName("locale_id");

                    b.Property<string>("LocaleName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("locale_name");

                    b.HasKey("LocaleId")
                        .HasName("pk_locale");

                    b.ToTable("locale", (string)null);

                    b.HasData(
                        new
                        {
                            LocaleId = 1,
                            LocaleName = "lahore"
                        },
                        new
                        {
                            LocaleId = 2,
                            LocaleName = "islamabad"
                        });
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

            modelBuilder.Entity("sproj.Data.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.Property<string>("RoleDescription")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("role_description");

                    b.HasKey("RoleId")
                        .HasName("pk_role");

                    b.ToTable("role", (string)null);

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            RoleDescription = "unregistered"
                        },
                        new
                        {
                            RoleId = 2,
                            RoleDescription = "employer"
                        },
                        new
                        {
                            RoleId = 3,
                            RoleDescription = "worker"
                        });
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

                    b.Property<string>("DrivingLicense")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("driving_license");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("full_name");

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

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.HasKey("UserId")
                        .HasName("pk_users");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_users_role_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("sproj.Data.UserPreferences", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int?>("JobLocale")
                        .HasColumnType("integer")
                        .HasColumnName("job_locale");

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

            modelBuilder.Entity("job_category_prefs", b =>
                {
                    b.HasOne("sproj.Data.JobCategory", null)
                        .WithMany()
                        .HasForeignKey("job_category_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_job_category_prefs_job_category_job_category_id");

                    b.HasOne("sproj.Data.UserPreferences", null)
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_job_category_prefs_user_preferences_user_id");
                });

            modelBuilder.Entity("job_experience_prefs", b =>
                {
                    b.HasOne("sproj.Data.JobExperience", null)
                        .WithMany()
                        .HasForeignKey("job_experience_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_job_experience_prefs_job_experience_job_experience_id");

                    b.HasOne("sproj.Data.UserPreferences", null)
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_job_experience_prefs_user_preferences_user_id");
                });

            modelBuilder.Entity("job_type_prefs", b =>
                {
                    b.HasOne("sproj.Data.JobType", null)
                        .WithMany()
                        .HasForeignKey("job_type_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_job_type_prefs_job_type_job_type_id");

                    b.HasOne("sproj.Data.UserPreferences", null)
                        .WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_job_type_prefs_user_preferences_user_id");
                });

            modelBuilder.Entity("sproj.Data.CnicVerification", b =>
                {
                    b.HasOne("sproj.Data.User", "User")
                        .WithOne("CnicVerifications")
                        .HasForeignKey("sproj.Data.CnicVerification", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cnic_verification_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("sproj.Data.Job", b =>
                {
                    b.HasOne("sproj.Data.JobCategory", "JobCategory")
                        .WithMany()
                        .HasForeignKey("JobCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_jobs_job_category_job_category_id");

                    b.HasOne("sproj.Data.JobExperience", "JobExperience")
                        .WithMany()
                        .HasForeignKey("JobExperienceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_jobs_job_experience_job_experience_id");

                    b.HasOne("sproj.Data.JobType", "JobType")
                        .WithMany()
                        .HasForeignKey("JobTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_jobs_job_type_job_type_id");

                    b.HasOne("sproj.Data.Locale", "Locale")
                        .WithMany()
                        .HasForeignKey("LocaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_jobs_locale_locale_id");

                    b.HasOne("sproj.Data.WorkerDetails", "WorkerDetails")
                        .WithOne()
                        .HasForeignKey("sproj.Data.Job", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_jobs_worker_details_user_id");

                    b.Navigation("JobCategory");

                    b.Navigation("JobExperience");

                    b.Navigation("JobType");

                    b.Navigation("Locale");

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
                    b.HasOne("sproj.Data.User", "User")
                        .WithOne("SmsVerifications")
                        .HasForeignKey("sproj.Data.SmsVerification", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_sms_verification_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("sproj.Data.User", b =>
                {
                    b.HasOne("sproj.Data.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_role_role_id");

                    b.Navigation("Role");
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
                    b.Navigation("CnicVerifications");

                    b.Navigation("SmsVerifications");

                    b.Navigation("UserPreferences");

                    b.Navigation("WorkerDetails");
                });
#pragma warning restore 612, 618
        }
    }
}