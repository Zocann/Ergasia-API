using Ergasia_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Data;

public class PrimaryDbContext(DbContextOptions<PrimaryDbContext> options) : DbContext(options)
{
    public DbSet<Employer> Employers { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<WorkerRating> WorkerRatings { get; set; }
    public DbSet<EmployerRating> EmployerRatings { get; set; }
    public DbSet<WorkerJob> WorkerJobs { get; set; }
    public DbSet<EmployerJob> EmployerJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkerJob>()
            .HasOne(wj => wj.Worker)
            .WithMany()
            .HasForeignKey(wj => wj.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<WorkerJob>()
            .HasOne(wj => wj.Job)
            .WithMany()
            .HasForeignKey(wj => wj.JobId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        modelBuilder.Entity<WorkerRating>()
            .HasOne(wr => wr.Worker)
            .WithMany()
            .HasForeignKey(r => r.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<WorkerRating>()
            .HasOne(wr => wr.Employer)
            .WithMany()
            .HasForeignKey(w => w.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        modelBuilder.Entity<EmployerRating>()
            .HasOne(er => er.Employer)
            .WithMany()
            .HasForeignKey(r => r.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<EmployerRating>()
            .HasOne(er => er.Worker)
            .WithMany()
            .HasForeignKey(w => w.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}