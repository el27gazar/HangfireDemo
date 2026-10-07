namespace HangfireDemo;

public class DailyMessageJob
{
    private readonly ILogger<DailyMessageJob> _logger;

    public DailyMessageJob(ILogger<DailyMessageJob> logger) => _logger = logger;

    public void Run()
    {
        var message = $"Hello from Hangfire! Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        Console.WriteLine(message);
        _logger.LogInformation(message);
    }
}
