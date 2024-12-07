﻿// <auto-generated />
using System;
using EventPlanning.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EventPlanning.Data.Migrations
{
    [DbContext(typeof(EventPlanningDbContext))]
    [Migration("20240705160359_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EventPlanning.Data.Entities.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventId"));

                    b.Property<int?>("AmountOfVacantPlaces")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("DressCode")
                        .HasColumnType("bit");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<int?>("ThemeId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EventId");

                    b.HasIndex("LocationId");

                    b.HasIndex("ThemeId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.Location", b =>
                {
                    b.Property<int>("LocationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LocationId"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LocationType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LocationId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.Participant", b =>
                {
                    b.Property<int>("ParticipantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ParticipantId"));

                    b.Property<string>("ParticipantName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ThemeId")
                        .HasColumnType("int");

                    b.HasKey("ParticipantId");

                    b.HasIndex("ThemeId");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.ParticipantEvent", b =>
                {
                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("ParticipantId")
                        .HasColumnType("int");

                    b.HasKey("EventId", "ParticipantId");

                    b.ToTable("ParticipantEvents");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.SubTheme", b =>
                {
                    b.Property<int?>("SubThemeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("SubThemeId"));

                    b.Property<string>("SubThemeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ThemeId")
                        .HasColumnType("int");

                    b.HasKey("SubThemeId");

                    b.HasIndex("ThemeId");

                    b.ToTable("SubThemes");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.Theme", b =>
                {
                    b.Property<int>("ThemeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ThemeId"));

                    b.Property<string>("ThemeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ThemeId");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.UserEvent", b =>
                {
                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool?>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.HasKey("EventId", "UserId");

                    b.ToTable("UserEvents");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.Event", b =>
                {
                    b.HasOne("EventPlanning.Data.Entities.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("EventPlanning.Data.Entities.Theme", "Theme")
                        .WithMany()
                        .HasForeignKey("ThemeId");

                    b.Navigation("Location");

                    b.Navigation("Theme");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.Participant", b =>
                {
                    b.HasOne("EventPlanning.Data.Entities.Theme", "Theme")
                        .WithMany()
                        .HasForeignKey("ThemeId");

                    b.Navigation("Theme");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.SubTheme", b =>
                {
                    b.HasOne("EventPlanning.Data.Entities.Theme", "Theme")
                        .WithMany("SubThemes")
                        .HasForeignKey("ThemeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Theme");
                });

            modelBuilder.Entity("EventPlanning.Data.Entities.Theme", b =>
                {
                    b.Navigation("SubThemes");
                });
#pragma warning restore 612, 618
        }
    }
}