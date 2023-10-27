﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProLibrary.Models;

#nullable disable

namespace ProLibrary.Migrations
{
    [DbContext(typeof(PROContext))]
    [Migration("20231027081625_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.24")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PRORepository.Models.DcMetadata", b =>
                {
                    b.Property<string>("Host")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("HOST");

                    b.Property<string>("ItemComment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("ITEM_COMMENT");

                    b.Property<string>("ItemContainer")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("ITEM_CONTAINER");

                    b.Property<int>("ItemId")
                        .HasColumnType("int")
                        .HasColumnName("ITEM_ID");

                    b.Property<string>("ItemName")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("ITEM_NAME");

                    b.Property<string>("ItemSource")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("ITEM_SOURCE");

                    b.Property<string>("ItemType")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("ITEM_TYPE");

                    b.Property<DateTime>("LastModifiedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("LAST_MODIFIED_AT");

                    b.Property<double?>("MaxVal")
                        .HasColumnType("float")
                        .HasColumnName("MAX_VAL");

                    b.Property<double?>("MinVal")
                        .HasColumnType("float")
                        .HasColumnName("MIN_VAL");

                    b.Property<string>("Orientation")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("ORIENTATION");

                    b.Property<double?>("Scaling")
                        .HasColumnType("float")
                        .HasColumnName("SCALING");

                    b.Property<string>("Sensor")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("SENSOR");

                    b.Property<string>("Unit")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("UNIT");

                    b.Property<double?>("UpdateCycle")
                        .HasColumnType("float")
                        .HasColumnName("UPDATE_CYCLE");

                    b.ToTable("DC_METADATA", "PRO");
                });
#pragma warning restore 612, 618
        }
    }
}