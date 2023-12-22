using Microsoft.EntityFrameworkCore;
using MISBack.Data.Entities;

namespace MISBack.Data;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Consultation> Consultation { get; set; }
    public DbSet<Diagnosis> Diagnosis { get; set; }
    public DbSet<Doctor> Doctor { get; set; }
    public DbSet<Icd10> Icd10 { get; set; }
    public DbSet<Inspection> Inspection { get; set; }
    public DbSet<Patient> Patient { get; set; }
    public DbSet<Speciality> Speciality { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, DbSet<Comment> comment,
        DbSet<Consultation> consultation, DbSet<Diagnosis> diagnosis, DbSet<Doctor> doctor, DbSet<Icd10> icd10,
        DbSet<Inspection> inspection, DbSet<Patient> patient, DbSet<Speciality> speciality)
        : base(options)
    {
        Comment = comment;
        Consultation = consultation;
        Diagnosis = diagnosis;
        Doctor = doctor;
        Icd10 = icd10;
        Inspection = inspection;
        Patient = patient;
        Speciality = speciality;
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

        base.OnModelCreating(modelBuilder);
    }
}