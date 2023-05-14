﻿// <auto-generated />
using System;
using DoorWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DoorWebAPI.Migrations
{
    [DbContext(typeof(DoorDbContext))]
    [Migration("20230514140001_D1")]
    partial class D1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DoorWebAPI.Models.Door", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("modifiedAt");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.Property<byte>("State")
                        .HasColumnType("tinyint unsigned")
                        .HasColumnName("state");

                    b.HasKey("Id");

                    b.ToTable("door", (string)null);
                });

            modelBuilder.Entity("DoorWebAPI.Models.Permission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<long>("DoorId")
                        .HasColumnType("bigint")
                        .HasColumnName("doorId");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("role");

                    b.HasKey("Id");

                    b.ToTable("permission", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
