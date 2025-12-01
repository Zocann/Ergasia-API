namespace Ergasia_API.Services;

//Basic exception service to log exceptions into files
public static class ExceptionService
{
    private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0], "Exceptions");
    
    public static void LogException(Exception exception)
    {
        Directory.CreateDirectory(LogDirectory);
        
        var fileName = $"{DateTime.UtcNow:dd-MM-yyyy}.txt";
        var filePath = Path.Combine(LogDirectory, fileName);
        
        var logEntry =
            $"[{DateTime.Now:HH:mm:ss}] {exception.GetType().Name}: {exception.Message}\n{exception.StackTrace}\n";
        
        if (File.Exists(filePath))
            logEntry = "\n\n" + logEntry;
        
        File.AppendAllText(filePath, logEntry);
    }
}