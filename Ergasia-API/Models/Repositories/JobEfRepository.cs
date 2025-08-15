using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class JobEfRepository(PrimaryDbContext context) : IJobRepository
{
    public Employer? GetEmployer(long jobId)
    {
        return context.EmployerJobs.Include(employerJob => employerJob.Employer)
            .FirstOrDefault(ej => ej.JobId == jobId)?.Employer;
    }

    public IEnumerable<Worker> GetAllWorkers(long jobId)
    {
       return context.WorkerJobs.Where(wj => wj.JobId == jobId)
            .Select(wj => wj.Worker).AsEnumerable();
    }

    public Job Add(Job job)
    {
        var lastJob = context.Jobs.Last();
        
        job.Id = lastJob.Id + 1;
        
        context.Add(job);
        context.SaveChanges();
        return job;
    }

    public Job? CreateCopy(int originalId)
    {
        var job = GetById(originalId);
        if (job == null) return null;

        var newJob = new Job(job.Name, DateTime.Now, job.Duration, job.AvilableWorkSpots);
        Add(newJob);
        return newJob;
    }

    public void Update(Job job)
    {
        context.Update(job);
        context.SaveChanges();
    }

    public Job? GetById(int id)
    {
        return context.Jobs.Find(id);
    }

    public IEnumerable<Job> GetAll()
    {
        return context.Jobs.AsEnumerable();
    }

    public IEnumerable<Job> GetAllActive()
    {
        return context.Jobs.Where(j => j.IsCurrent == true).AsEnumerable();
    }

    public IEnumerable<Job> GetAllByMonthOfBegin(int month)
    {
        return context.Jobs.Where(j => j.DateOfBegin.Month == month && j.DateOfBegin.Year == DateTime.Now.Year)
            .AsEnumerable();
    }

    public IEnumerable<Job> GetAllByRating(float rating)
    {
        return context.WorkerJobs.Where(wj => wj.NumericalRating >= rating).Select(wj => wj.Job).AsEnumerable();
    }

    public bool Delete(int id)
    {
        var job = GetById(id);
        if (job == null) return false;
        
        context.Jobs.Remove(job);
        context.SaveChanges();
        return true;
    }
}