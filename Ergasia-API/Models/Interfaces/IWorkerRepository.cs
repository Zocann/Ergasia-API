namespace Ergasia_API.Models.Interfaces;

public interface IWorkerRepository
{
    public Job? GetCurrentJob(int workerId);
    public IEnumerable<Job> GetPastJobs(int workerId);
    public IEnumerable<Job> GetAllJobs(int workerId);
    public Employer? GetCurrentEmployer(int workerId);
    public IEnumerable<Employer>? GetPastEmployers(int workerId);
    
    public Worker Add(Worker worker);
    public Worker? Update(Worker worker);
    public Worker? GetById(int id);
    public IEnumerable<Worker> GetAll();
    public IEnumerable<Worker> GetAllActive();
    public IEnumerable<int> GetAllRatingsFromWorker(int workerId);
    public double GetAverageRating(int workerId);
    public IEnumerable<Worker> GetAllByRating(float rating);
    public bool Delete(int id);
}