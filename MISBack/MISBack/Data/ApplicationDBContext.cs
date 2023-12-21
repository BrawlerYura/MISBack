using Microsoft.EntityFrameworkCore;
using MISBack.Data.Entities;

namespace MISBack.Data;

public class ApplicationDBContext : DbContext
{
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Consultation> Consultation { get; set; }
    public DbSet<Diagnosis> Diagnosis { get; set; }
    public DbSet<Doctor> Doctor { get; set; }
    public DbSet<Icd10> Icd10 { get; set; }
    public DbSet<Inspection> Inspection { get; set; }
    public DbSet<Patient> Patient { get; set; }
    public DbSet<Speciality> Speciality { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
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
        
        base.OnModelCreating(modelBuilder);
    }
}