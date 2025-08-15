namespace Ergasia_API.Models.Interfaces;

public interface IJobRepository
{
    public Employer? GetEmployer(long jobId);
    public IEnumerable<Worker>? GetAllWorkers(long jobId);
    public Job Add(Job job);
    public Job? CreateCopy(int originalId);
    public void Update(Job job);
    public Job? GetById(int id);
    public IEnumerable<Job> GetAll();
    public IEnumerable<Job> GetAllActive();
    public IEnumerable<Job> GetAllByMonthOfBegin(int month);
    public IEnumerable<Job> GetAllByRating(float rating);
    public bool Delete(int id);
}