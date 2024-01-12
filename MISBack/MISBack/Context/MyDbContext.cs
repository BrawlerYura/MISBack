using System;
using System.Collections.Generic;
using MISBack.Migrations;
using Microsoft.EntityFrameworkCore;
using MISBack.Data.Entities;

namespace MISBack.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Consultation> Consultations { get; set; }

    public virtual DbSet<Diagnosis> Diagnoses { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Icd10> Icd10 { get; set; }

    public virtual DbSet<Inspection> Inspections { get; set; }

    public virtual DbSet<InspectionDiagnosis> InspectionDiagnoses { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Speciality> Specialities { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("host=localhost;port=5432;database=MISBack;username=postgres;password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.IsRootComment).HasDefaultValue(false);
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.ToTable("Consultation");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.ToTable("Diagnosis");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Password).HasDefaultValueSql("''::text");
        });

        modelBuilder.Entity<Icd10>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("icd10");

            entity.Property(e => e.Actual).HasColumnName("actual");
            entity.Property(e => e.AddlCode).HasColumnName("addl_code");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdParent).HasColumnName("id_parent");
            entity.Property(e => e.MkbCode)
                .HasMaxLength(255)
                .HasColumnName("mkb_code");
            entity.Property(e => e.MkbName).HasColumnName("mkb_name");
            entity.Property(e => e.RecCode)
                .HasMaxLength(255)
                .HasColumnName("rec_code");
        });

        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.ToTable("Inspection");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<InspectionDiagnosis>(entity =>
        {
            entity.HasKey(e => new { e.InspectionId, e.DiagnosisId });

            entity.ToTable("InspectionDiagnosis");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Speciality>(entity =>
        {
            entity.ToTable("Speciality");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.InvalidToken);

            entity.ToTable("Token");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
