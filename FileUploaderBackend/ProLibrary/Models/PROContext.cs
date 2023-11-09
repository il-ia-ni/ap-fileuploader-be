using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public virtual DbSet<TmiUser> TmiUsers { get; set; } = null!;

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
                entity.HasKey(e => e.ItemId)
                    .HasName("DC_METADATA_PK");

                entity.ToTable("DC_METADATA", "PRO");

                entity.Property(e => e.ItemId)
                    .HasPrecision(10)
                    .UseIdentityColumn()
                    .HasColumnName("ITEM_ID");

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

            modelBuilder.Entity<TmiUser>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("TMI_USER_PK");

                entity.ToTable("TMI_USER", "PRO");

                entity.Property(e => e.Username)
                    .HasMaxLength(40)
                    .HasColumnName("USERNAME");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(40)
                    .HasColumnName("FIRST_NAME");

                entity.Property(e => e.LandingPage)
                    .HasMaxLength(40)
                    .HasColumnName("LANDING_PAGE");

                entity.Property(e => e.LastModifiedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("LAST_MODIFIED_AT");

                entity.Property(e => e.LastName)
                    .HasMaxLength(40)
                    .HasColumnName("LAST_NAME");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(512)
                    .HasColumnName("PASSWORD_HASH");

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(512)
                    .HasColumnName("PASSWORD_SALT");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(40)
                    .HasColumnName("ROLE_NAME");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
