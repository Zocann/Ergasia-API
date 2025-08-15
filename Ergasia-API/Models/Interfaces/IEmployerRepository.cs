namespace Ergasia_API.Models.Interfaces;

public interface IEmployerRepository
{
    public IEnumerable<Job>? GetAllJobs(int employerId);
    public IEnumerable<Job>? GetCurrentOrPastJobs(int employerId, bool getPastJobs = false);
    public IEnumerable<Worker>? GetAllWorkers(int employerId);
    public IEnumerable<Worker> GetCurrentOrPastWorkers(int employerId, bool getPastWorkers = false);
    public Employer Add(Employer employer);
    public Employer Update(Employer employer);
    public Employer? GetById(int id);
    public IEnumerable<Employer> GetAll();
    public IEnumerable<Employer> GetAllActive();
    public IEnumerable<int>? GetAllRatingsFromEmployer(int employerId);
    public double GetAverageRating(int employerId);
    public IEnumerable<Employer> GetAllByRating(double rating);
    public bool Delete(int id);
}