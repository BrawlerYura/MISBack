using Microsoft.EntityFrameworkCore;
using MISBack.Data.Entities;
using MISBack.Migrations;
using Comment = MISBack.Data.Entities.Comment;
using Consultation = MISBack.Data.Entities.Consultation;
using Diagnosis = MISBack.Data.Entities.Diagnosis;
using Doctor = MISBack.Data.Entities.Doctor;
using EmailingLogs = MISBack.Data.Entities.EmailingLogs;
using Inspection = MISBack.Data.Entities.Inspection;
using InspectionDiagnosis = MISBack.Data.Entities.InspectionDiagnosis;
using Patient = MISBack.Data.Entities.Patient;
using Speciality = MISBack.Data.Entities.Speciality;
using Token = MISBack.Data.Entities.Token;

namespace MISBack.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Consultation> Consultation { get; set; }
    public DbSet<Diagnosis> Diagnosis { get; set; }
    public DbSet<Doctor> Doctor { get; set; }
    public DbSet<Icd10> Icd10 { get; set; }
    public DbSet<Inspection> Inspection { get; set; }
    public DbSet<Patient> Patient { get; set; }
    public DbSet<Speciality> Speciality { get; set; }
    public DbSet<Token> Token { get; set; }
    public DbSet<InspectionDiagnosis> InspectionDiagnosis { get; set; }
    
    public DbSet<EmailingLogs> EmailingLogs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>().HasKey(x => x.Id);
        modelBuilder.Entity<Consultation>().HasKey(x => x.Id);
        modelBuilder.Entity<Diagnosis>().HasKey(x => x.Id);
        modelBuilder.Entity<Doctor>().HasKey(x => x.Id);
        modelBuilder.Entity<Icd10>().HasKey(x => x.Id);
        modelBuilder.Entity<Inspection>().HasKey(x => x.Id);
        modelBuilder.Entity<Patient>().HasKey(x => x.Id);
        modelBuilder.Entity<Speciality>().HasKey(x => x.Id);
        modelBuilder.Entity<Token>().HasKey(x => x.InvalidToken);
        modelBuilder.Entity<EmailingLogs>().HasKey(x => x.Id);
        modelBuilder.Entity<InspectionDiagnosis>().HasKey(x => new { x.InspectionId, x.DiagnosisId });

        base.OnModelCreating(modelBuilder);
    }
}