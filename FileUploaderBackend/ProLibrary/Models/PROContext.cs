using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace ProLibrary.Models
{
    public partial class PROContext : DbContext
    {
        public PROContext()
        {
        }

        public PROContext(DbContextOptions<PROContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DcMetadata> DcMetadata { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString!);  // See partial class in ModelExtensions
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DcMetadata>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DC_METADATA", "PRO");

                entity.Property(e => e.Host)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("HOST");

                entity.Property(e => e.ItemComment)
                    .HasMaxLength(1000)
                    .HasColumnName("ITEM_COMMENT");

                entity.Property(e => e.ItemContainer)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_CONTAINER");

                entity.Property(e => e.ItemId).HasColumnName("ITEM_ID");

                entity.Property(e => e.ItemName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NAME");

                entity.Property(e => e.ItemSource)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_SOURCE");

                entity.Property(e => e.ItemType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_TYPE");

                entity.Property(e => e.LastModifiedAt).HasColumnName("LAST_MODIFIED_AT");

                entity.Property(e => e.MaxVal).HasColumnName("MAX_VAL");

                entity.Property(e => e.MinVal).HasColumnName("MIN_VAL");

                entity.Property(e => e.Orientation)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ORIENTATION");

                entity.Property(e => e.Scaling).HasColumnName("SCALING");

                entity.Property(e => e.Sensor)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SENSOR");

                entity.Property(e => e.Unit)
                    .HasMaxLength(100)
                    .HasColumnName("UNIT");

                entity.Property(e => e.UpdateCycle).HasColumnName("UPDATE_CYCLE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
