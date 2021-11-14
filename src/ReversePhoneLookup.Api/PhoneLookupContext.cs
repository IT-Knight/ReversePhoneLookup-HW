using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ReversePhoneLookup.Api.Models.Entities;

#nullable disable

namespace ReversePhoneLookup.Api
{
    public partial class PhoneLookupContext : DbContext
    {
        public PhoneLookupContext()
        {
        }

        public PhoneLookupContext(DbContextOptions<PhoneLookupContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("contact");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.PhoneId).HasColumnName("phone_id");

                entity.HasOne(d => d.Phone)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.PhoneId)
                    .HasConstraintName("contact_phone_id_fkey");
            });

            modelBuilder.Entity<Operator>(entity =>
            {
                entity.ToTable("operator");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Mcc)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("mcc");

                entity.Property(e => e.Mnc)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("mnc");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Phone>(entity =>
            {
                entity.ToTable("phone");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OperatorId).HasColumnName("operator_id");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("value");

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(d => d.OperatorId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("phone_operator_id_fkey");
            });

            modelBuilder.HasSequence("contact_id_seq");

            modelBuilder.HasSequence("operator_id_seq");

            modelBuilder.HasSequence("phone_id_seq");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
