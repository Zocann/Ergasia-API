using Ergasia_API.DTOs.User;

namespace Ergasia_API.DTOs.Worker;

public class WorkerDto : UserDto
{
    public int? MinimalSalary { get; set; }
    public string? Description { get; set; }
}