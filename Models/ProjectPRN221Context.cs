﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectPRN221.Models
{
    public partial class ProjectPRN221Context : DbContext
    {
        public ProjectPRN221Context()
        {
        }

        public ProjectPRN221Context(DbContextOptions<ProjectPRN221Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Input> Inputs { get; set; } = null!;
        public virtual DbSet<InputInfo> InputInfos { get; set; } = null!;
        public virtual DbSet<Object> Objects { get; set; } = null!;
        public virtual DbSet<Output> Outputs { get; set; } = null!;
        public virtual DbSet<OutputInfo> OutputInfos { get; set; } = null!;
        public virtual DbSet<Suplier> Supliers { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =localhost; database = ProjectPRN221;uid=truong;pwd=1234;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.ContractDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<Input>(entity =>
            {
                entity.ToTable("Input");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.DateInput).HasColumnType("datetime");
            });

            modelBuilder.Entity<InputInfo>(entity =>
            {
                entity.ToTable("InputInfo");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.IdInput).HasMaxLength(128);

                entity.Property(e => e.IdObject).HasMaxLength(128);

                entity.Property(e => e.InputPrice).HasDefaultValueSql("((0))");

                entity.Property(e => e.OutputPrice).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdInputNavigation)
                    .WithMany(p => p.InputInfos)
                    .HasForeignKey(d => d.IdInput)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InputInfo__IdInp__4AB81AF0");

                entity.HasOne(d => d.IdObjectNavigation)
                    .WithMany(p => p.InputInfos)
                    .HasForeignKey(d => d.IdObject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InputInfo__Statu__49C3F6B7");
            });

            modelBuilder.Entity<Object>(entity =>
            {
                entity.ToTable("Object");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.Qrcode).HasColumnName("QRCode");

                entity.HasOne(d => d.IdSuplierNavigation)
                    .WithMany(p => p.Objects)
                    .HasForeignKey(d => d.IdSuplier)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Object__IdSuplie__3E52440B");

                entity.HasOne(d => d.IdUnitNavigation)
                    .WithMany(p => p.Objects)
                    .HasForeignKey(d => d.IdUnit)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Object__BarCode__3D5E1FD2");
            });

            modelBuilder.Entity<Output>(entity =>
            {
                entity.ToTable("Output");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.DateOutput).HasColumnType("datetime");
            });

            modelBuilder.Entity<OutputInfo>(entity =>
            {
                entity.ToTable("OutputInfo");

                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.IdObject).HasMaxLength(128);

                entity.Property(e => e.IdOutputInfo).HasMaxLength(128);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.OutputInfo)
                    .HasForeignKey<OutputInfo>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OutputInfo__Id__5165187F");

                entity.HasOne(d => d.IdCustomerNavigation)
                    .WithMany(p => p.OutputInfos)
                    .HasForeignKey(d => d.IdCustomer)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OutputInf__IdCus__5070F446");

                entity.HasOne(d => d.IdObjectNavigation)
                    .WithMany(p => p.OutputInfos)
                    .HasForeignKey(d => d.IdObject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OutputInf__Statu__4F7CD00D");
            });

            modelBuilder.Entity<Suplier>(entity =>
            {
                entity.ToTable("Suplier");

                entity.Property(e => e.ContractDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserName).HasMaxLength(100);

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Users__IdRole__4316F928");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
