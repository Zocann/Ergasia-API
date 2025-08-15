using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkersController(IWorkerRepository repository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<Worker> Get()
    {
        return repository.GetAll();
    }
    
    [HttpGet("{id:int}")]
    public Worker? Get(int id)
    {
        var worker = repository.GetById(id);
        
        if (worker != null) return worker;
        
        ModelState.AddModelError("Id", "Worker with this Id does not exist");
        Response.StatusCode = 404;
        return null;
    }
    
    [HttpPost]
    public Worker? Post(Worker worker)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return null;
        }

        Response.StatusCode = 201;
        Response.Headers.Location = $"/Workers/{worker.Id}";
        return repository.Add(worker);
    }

    [HttpDelete("{id:int}")]
    public void Delete(int id)
    {
        var deleted = repository.Delete(id);

        Response.StatusCode = deleted ? 204 : 404;
    }
    
    [HttpPut("{id:int}")]
    public Worker? Update(Worker worker, int id)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return null;
        }

        if (id != worker.Id)
        {
            Response.StatusCode = 405; 
            return null;
        }
        
        var updatedWorker = repository.Update(worker);
        
        Response.StatusCode = updatedWorker == null ? 404 : 200;
        return updatedWorker;
    }
    
}