﻿// <auto-generated />
using System;
using App.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace App.Database.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230319130340_InitialDb")]
    partial class InitialDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("App.Domain.alienEntity", b =>
                {
                    b.Property<Guid>("id")
                        .HasColumnType("char(36)")
                        .HasColumnOrder(1)
                        .HasComment("คีร์ของข้อมูล");

                    b.Property<DateTime?>("created")
                        .HasColumnType("datetime(6)")
                        .HasColumnOrder(200)
                        .HasComment("เวลาสร้าง");

                    b.Property<bool?>("isActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnOrder(202)
                        .HasComment("ใช้งานได้หรือไม่");

                    b.Property<string>("name")
                        .HasMaxLength(4000)
                        .HasColumnType("varchar(4000)")
                        .HasColumnOrder(2)
                        .HasComment("Alien Name");

                    b.Property<string>("origin_planet")
                        .HasMaxLength(4000)
                        .HasColumnType("varchar(4000)")
                        .HasColumnOrder(4)
                        .HasComment("Origin Planet");

                    b.Property<string>("species")
                        .HasMaxLength(4000)
                        .HasColumnType("varchar(4000)")
                        .HasColumnOrder(3)
                        .HasComment("Species");

                    b.Property<DateTime?>("updated")
                        .HasColumnType("datetime(6)")
                        .HasColumnOrder(201)
                        .HasComment("เวลาปรับปรุงล่าสุด");

                    b.HasKey("id");

                    b.ToTable("aliens");
                });

            modelBuilder.Entity("App.Domain.sightingEntity", b =>
                {
                    b.Property<Guid>("id")
                        .HasColumnType("char(36)")
                        .HasColumnOrder(1)
                        .HasComment("คีร์ของข้อมูล");

                    b.Property<Guid?>("alien_id")
                        .HasColumnType("char(36)")
                        .HasColumnOrder(2)
                        .HasComment("Alien");

                    b.Property<DateTime?>("created")
                        .HasColumnType("datetime(6)")
                        .HasColumnOrder(200)
                        .HasComment("เวลาสร้าง");

                    b.Property<DateTime?>("found_date")
                        .HasColumnType("datetime(6)")
                        .HasColumnOrder(3)
                        .HasComment("Found Date");

                    b.Property<bool?>("isActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnOrder(202)
                        .HasComment("ใช้งานได้หรือไม่");

                    b.Property<string>("location")
                        .HasColumnType("longtext")
                        .HasColumnOrder(4)
                        .HasComment("Location");

                    b.Property<DateTime?>("updated")
                        .HasColumnType("datetime(6)")
                        .HasColumnOrder(201)
                        .HasComment("เวลาปรับปรุงล่าสุด");

                    b.Property<string>("witness")
                        .HasColumnType("longtext")
                        .HasColumnOrder(5)
                        .HasComment("Witness");

                    b.HasKey("id");

                    b.HasIndex("alien_id");

                    b.ToTable("sightings");
                });

            modelBuilder.Entity("App.Domain.sightingEntity", b =>
                {
                    b.HasOne("App.Domain.alienEntity", "alien_alien_id")
                        .WithMany("sightings")
                        .HasForeignKey("alien_id");

                    b.Navigation("alien_alien_id");
                });

            modelBuilder.Entity("App.Domain.alienEntity", b =>
                {
                    b.Navigation("sightings");
                });
#pragma warning restore 612, 618
        }
    }
}