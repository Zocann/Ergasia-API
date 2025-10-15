using Ergasia_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Data;

public class PrimaryDbContext(DbContextOptions<PrimaryDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Employer> Employers { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<WorkerRating> WorkerRatings { get; set; }
    public DbSet<EmployerRating> EmployerRatings { get; set; }
    public DbSet<WorkerJob> WorkerJobs { get; set; }
    public DbSet<WorkerJobRequest> WorkerJobRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "employer-id", Name = "Employer", NormalizedName = "EMPLOYER" },
            new IdentityRole { Id = "worker-id", Name = "Worker", NormalizedName = "WORKER" }, 
            new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN"}
        );
            
        
        // Configure Worker-Employer distinctions
        modelBuilder.Entity<Worker>().HasBaseType<User>().ToTable("Workers");
        modelBuilder.Entity<Employer>().HasBaseType<User>().ToTable("Employers");
        
        modelBuilder.Entity<Job>()
            .HasOne(j => j.Employer)
            .WithMany()
            .HasForeignKey(j => j.EmployerId);
        
        
        //Defining composed key
        modelBuilder.Entity<WorkerJob>()
            .HasKey(wj => new { wj.WorkerId, wj.JobId });
        
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

        
        modelBuilder.Entity<WorkerJobRequest>()
            .HasKey(wjr => new { wjr.WorkerId, wjr.JobId });
        
        modelBuilder.Entity<WorkerJobRequest>()
            .HasOne(wj => wj.Worker)
            .WithMany()
            .HasForeignKey(wj => wj.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkerJobRequest>()
            .HasOne(wj => wj.Job)
            .WithMany()
            .HasForeignKey(wj => wj.JobId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<WorkerRating>()
            .HasKey(wr => new { wr.WorkerId, wr.EmployerId });
        
        modelBuilder.Entity<WorkerRating>()
            .HasOne(wr => wr.Worker)
            .WithMany()
            .HasForeignKey(wr => wr.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkerRating>()
            .HasOne(wr => wr.Employer)
            .WithMany()
            .HasForeignKey(wr => wr.EmployerId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<EmployerRating>()
            .HasKey(wr => new { wr.WorkerId, wr.EmployerId });
        
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