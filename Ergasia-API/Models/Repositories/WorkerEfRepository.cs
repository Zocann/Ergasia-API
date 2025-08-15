using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;

namespace Ergasia_API.Models.Repositories;

public class WorkerEfRepository(PrimaryDbContext context, IJobRepository jobRepository) : IWorkerRepository
{
    public IEnumerable<Job> GetAllJobs(int workerId)
    {
        return context.WorkerJobs.Where(j => j.WorkerId == workerId)
            .Select(wj => wj.Job).AsEnumerable();
    }
    
    public Job? GetCurrentJob(int workerId)
    {
        var jobs = GetAllJobs(workerId);
        
        return jobs.FirstOrDefault(j => j.IsCurrent);
    }

    public IEnumerable<Job> GetPastJobs(int workerId)
    {
        var jobs = GetAllJobs(workerId);
        
        return jobs.Where(j => j.IsCurrent == false);
    }
    
    public Employer? GetCurrentEmployer(int workerId)
    {
        var job = GetCurrentJob(workerId);
        
        return job == null ? null : jobRepository.GetEmployer(job.Id);
    }

    public IEnumerable<Employer> GetPastEmployers(int workerId)
    {
        var pastJobs = GetPastJobs(workerId);
        
        List<Employer> pastEmployers = [];

        foreach (var pastJob in pastJobs)
        {
            var pastEmployer = jobRepository.GetEmployer(pastJob.Id);
            if (pastEmployer != null) pastEmployers.Add(pastEmployer);
        }

        return pastEmployers.AsEnumerable();
    }
    
    public Worker Add(Worker worker)
    {
        worker.Id = 0;
        
        var entityWorker = context.Workers.Add(worker);
        context.SaveChanges(); 
        
        worker.Id = entityWorker.Entity.Id;
        return worker;
    }

    public Worker? Update(Worker worker)
    {
        if (context.Workers.Find(worker.Id) == null) return null;
        
        context.Workers.Update(worker);
        context.SaveChanges();
        return worker;
    }
    
    public Worker? GetById(int id)
    {
        return context.Workers.Find(id);
    }

    public IEnumerable<Worker> GetAll()
    {
        return context.Workers.AsEnumerable();
    }
    
    public IEnumerable<Worker> GetAllActive()
    {
        return context.Workers.Where(w => w.IsActive == true).AsEnumerable();
    }

    public IEnumerable<int> GetAllRatingsFromWorker(int workerId)
    {
        var enumerable = context.WorkerRatings
            .Where(wj => wj.WorkerId == workerId).AsEnumerable();

        var workerRating = enumerable.ToList();
        
        return workerRating.Select(wj => wj.NumericalRating);
    }
    
    public double GetAverageRating(int workerId)
    {
        var enumerable = GetAllRatingsFromWorker(workerId);


        var ratings = enumerable.ToList();
        
        return ratings.Count != 0 ? -1 : ratings.Average();
    }

    public IEnumerable<Worker> GetAllByRating(float rating)
    {
        var workers = GetAll();
        
        return workers.Where(w => GetAverageRating(w.Id) >= rating).AsEnumerable();
    }

    public bool Delete(int id)
    {
        var worker = context.Workers.Find(id);
        if (worker == null) return false;
        
        context.Workers.Remove(worker);
        context.SaveChanges();
        
        return true;
    }
}