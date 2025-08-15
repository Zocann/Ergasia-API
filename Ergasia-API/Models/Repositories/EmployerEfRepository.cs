using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;

namespace Ergasia_API.Models.Repositories;

public class EmployerEfRepository(PrimaryDbContext context, IJobRepository jobRepository) : IEmployerRepository
{
    public IEnumerable<Job> GetAllJobs(int employerId)
    {
        return context.EmployerJobs.Where(ej => ej.EmployerId == employerId).Select(ej => ej.Job).AsEnumerable();
    }
    
    public IEnumerable<Job> GetCurrentOrPastJobs(int employerId, bool getPastJobs = false)
    {
        var jobs = GetAllJobs(employerId);

        return jobs.Where(j => j.IsCurrent == !getPastJobs).AsEnumerable();
    }

    public IEnumerable<Worker> GetAllWorkers(int employerId)
    {
        var jobs = GetAllJobs(employerId);

        List<Worker> allWorkers = [];
        foreach (var jobId in jobs.Select(j => j.Id))
        {
            var workers = jobRepository.GetAllWorkers(jobId);
            if (workers == null) continue;
            
            allWorkers.AddRange(workers);
        }
        return allWorkers;
    }
    
    public IEnumerable<Worker> GetCurrentOrPastWorkers(int employerId, bool getPastWorkers = false)
    {
        var jobs = GetCurrentOrPastJobs(employerId, getPastWorkers);
        
        List<Worker> allWorkers = [];
        foreach (var jobId in jobs.Select(j => j.Id))
        {
            var workers = jobRepository.GetAllWorkers(jobId);
            if (workers == null) continue;
            
            allWorkers.AddRange(workers);
        }
        return allWorkers;
    }

    public Employer Add(Employer employer)
    {
        var lastEmployer = context.Employers.Last();
        
        employer.Id = lastEmployer.Id + 1;
        
        context.Employers.Add(employer);
        context.SaveChanges();
        return employer;
    }

    public Employer Update(Employer employer)
    {
        context.Employers.Update(employer);
        context.SaveChanges();
        return employer;
    }

    public Employer? GetById(int id)
    {
        return context.Employers.Find(id);
    }

    public IEnumerable<Employer> GetAll()
    {
        return context.Employers.AsEnumerable();
    }

    public IEnumerable<Employer> GetAllActive()
    {
        return context.Employers.Where(w => w.IsActive == true).AsEnumerable();
    }

    public IEnumerable<int>? GetAllRatingsFromEmployer(int employerId)
    {
        var enumerable = context.EmployerRatings
            .Where(ej => ej.EmployerId == employerId).AsEnumerable();

        var employerRatings = enumerable.ToList();
        
        return employerRatings.Count == 0 ? null : employerRatings.Select(ej => ej.NumericalRating);
    }

    public double GetAverageRating(int employerId)
    {
        var ratings = GetAllRatingsFromEmployer(employerId);
        
        return ratings == null ? -1 : ratings.Average();
    }

    public IEnumerable<Employer> GetAllByRating(double rating)
    {
        var employers = GetAll();
        
        return employers.Where(e => GetAverageRating(e.Id) >= rating).AsEnumerable();
    }

    public bool Delete(int id)
    {
        var employer = context.Employers.Find(id);
        if(employer == null) return false;
         
        context.Employers.Remove(employer);
        context.SaveChanges();
        return true;
    }
}