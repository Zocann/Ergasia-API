using Ergasia_API.Models;

namespace Ergasia_API.Data;

public static class DbInitializer
{
    public static void Seed(PrimaryDbContext context)
    {
        if (!context.Employers.Any())
        {
            context.Employers.AddRange(
                new Employer("Douglas Farms", "douglas@gmail.com", "1234"),
                new Employer("Johns Apples", "john@gmail.com", "1234"),
                new Employer("Karen Fish", "karen@gmail.com", "1234")
            );
        }

        if (!context.Workers.Any())
        {
            context.Workers.AddRange(
                new Worker("Charles", "Maron", "charle@gmail.com", "1234"),
                new Worker("Herold", "Dwayne", "herold@gmail.com", "1234"),
                new Worker("Teamour", "Manley", "teamour@gmail.com", "1234"),
                new Worker("John", "Doe", "john@gmail.com", "1234"),
                new Worker("Barney", "Stinson", "barney@gmail.com", "1234")
            );
        }

        if (!context.Jobs.Any())
        {
        context.Jobs.AddRange(
                new Job("Collecting Apples", DateTime.Now.AddMonths(2), 20, 3),
                new Job("Farming", DateTime.Now.AddMonths(4).AddDays(10), 31, 4),
                new Job("Fishing", DateTime.Now.AddMonths(10).AddDays(1), 15, 2)
                );
        }
        context.SaveChanges();
    }
}