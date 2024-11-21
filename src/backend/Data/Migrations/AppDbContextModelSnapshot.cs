﻿// <auto-generated />
using System;
using System.Collections.Generic;
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

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_category", new[] { "babysitting", "cleaning", "cooking", "driving", "gardening", "laundry", "pet_care", "security_guard" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_experience", new[] { "beginner", "expert", "intermediate" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_gender", new[] { "couple", "female", "male" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "job_type", new[] { "one_shot", "permanent_hire" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "role", new[] { "employer", "unregistered", "worker" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_gender", new[] { "female", "male" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("sproj.Data.CnicVerification", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("IdImage")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int>("IdType")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("CnicVerification");
                });

            modelBuilder.Entity("sproj.Data.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("JobId"));

                    b.Property<JobCategory>("JobCategory")
                        .HasColumnType("job_category");

                    b.Property<JobExperience>("JobExperience")
                        .HasColumnType("job_experience");

                    b.Property<JobGender>("JobGender")
                        .HasColumnType("job_gender");

                    b.Property<JobType>("JobType")
                        .HasColumnType("job_type");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("WageRate")
                        .HasColumnType("integer");

                    b.Property<int?>("WorkerDetailsUserId")
                        .HasColumnType("integer");

                    b.HasKey("JobId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.HasIndex("WorkerDetailsUserId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("sproj.Data.PastJob", b =>
                {
                    b.Property<int>("PastJobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PastJobId"));

                    b.Property<string>("Comments")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("EmployerPhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<JobCategory>("JobCategory")
                        .HasColumnType("job_category");

                    b.Property<JobGender>("JobGender")
                        .HasColumnType("job_gender");

                    b.Property<JobType>("JobType")
                        .HasColumnType("job_type");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int?>("Rating")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int?>("WorkerDetailsUserId")
                        .HasColumnType("integer");

                    b.HasKey("PastJobId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.HasIndex("WorkerDetailsUserId");

                    b.ToTable("PastJobs");
                });

            modelBuilder.Entity("sproj.Data.PermanentJob", b =>
                {
                    b.Property<int>("JobId")
                        .HasColumnType("integer");

                    b.Property<int>("TrialPeriod")
                        .HasColumnType("integer");

                    b.HasKey("JobId");

                    b.ToTable("PermanentJob");
                });

            modelBuilder.Entity("sproj.Data.SmsVerification", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("VerificationCode")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)");

                    b.HasKey("UserId");

                    b.ToTable("SmsVerification");
                });

            modelBuilder.Entity("sproj.Data.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<DateOnly>("Birthdate")
                        .HasColumnType("date");

                    b.Property<string>("CnicNumber")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<int?>("CoupleUserId")
                        .HasColumnType("integer");

                    b.Property<string>("DrivingLicense")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<UserGender>("Gender")
                        .HasColumnType("user_gender");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)");

                    b.Property<Role>("Role")
                        .HasColumnType("role");

                    b.HasKey("UserId");

                    b.HasIndex("CoupleUserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("sproj.Data.UserPreferences", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.PrimitiveCollection<List<JobCategory>>("JobCategories")
                        .IsRequired()
                        .HasColumnType("job_category[]");

                    b.PrimitiveCollection<List<JobExperience>>("JobExperiences")
                        .IsRequired()
                        .HasColumnType("job_experience[]");

                    b.Property<string>("JobLocale")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.PrimitiveCollection<List<JobType>>("JobTypes")
                        .IsRequired()
                        .HasColumnType("job_type[]");

                    b.HasKey("UserId");

                    b.ToTable("UserPreferences");
                });

            modelBuilder.Entity("sproj.Data.WorkerDetails", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("WorkerDetails");
                });

            modelBuilder.Entity("sproj.Data.CnicVerification", b =>
                {
                    b.HasOne("sproj.Data.User", null)
                        .WithOne("CnicVerifications")
                        .HasForeignKey("sproj.Data.CnicVerification", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("sproj.Data.Job", b =>
                {
                    b.HasOne("sproj.Data.WorkerDetails", "WorkerDetails")
                        .WithOne()
                        .HasForeignKey("sproj.Data.Job", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("sproj.Data.WorkerDetails", null)
                        .WithMany("Jobs")
                        .HasForeignKey("WorkerDetailsUserId");

                    b.Navigation("WorkerDetails");
                });

            modelBuilder.Entity("sproj.Data.PastJob", b =>
                {
                    b.HasOne("sproj.Data.WorkerDetails", "WorkerDetails")
                        .WithOne()
                        .HasForeignKey("sproj.Data.PastJob", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("sproj.Data.WorkerDetails", null)
                        .WithMany("PastJobs")
                        .HasForeignKey("WorkerDetailsUserId");

                    b.Navigation("WorkerDetails");
                });

            modelBuilder.Entity("sproj.Data.PermanentJob", b =>
                {
                    b.HasOne("sproj.Data.Job", null)
                        .WithOne("PermanentJobDetails")
                        .HasForeignKey("sproj.Data.PermanentJob", "JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("sproj.Data.SmsVerification", b =>
                {
                    b.HasOne("sproj.Data.User", null)
                        .WithOne("SmsVerifications")
                        .HasForeignKey("sproj.Data.SmsVerification", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("sproj.Data.User", b =>
                {
                    b.HasOne("sproj.Data.User", "Couple")
                        .WithMany()
                        .HasForeignKey("CoupleUserId");

                    b.Navigation("Couple");
                });

            modelBuilder.Entity("sproj.Data.UserPreferences", b =>
                {
                    b.HasOne("sproj.Data.User", null)
                        .WithOne("UserPreferences")
                        .HasForeignKey("sproj.Data.UserPreferences", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("sproj.Data.WorkerDetails", b =>
                {
                    b.HasOne("sproj.Data.User", "User")
                        .WithOne("WorkerDetails")
                        .HasForeignKey("sproj.Data.WorkerDetails", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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

            modelBuilder.Entity("sproj.Data.WorkerDetails", b =>
                {
                    b.Navigation("Jobs");

                    b.Navigation("PastJobs");
                });
#pragma warning restore 612, 618
        }
    }
}
