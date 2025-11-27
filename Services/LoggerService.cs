namespace BudgetTracker.Services;

// Handles event-driven logging.
// Logging failures never interrupt application flow.
public class LoggerService
{
    private readonly string _logFilePath;

    public LoggerService(string logFilePath)
    {
        _logFilePath = logFilePath;

        var directory = Path.GetDirectoryName(_logFilePath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }

    // Subscribes to transaction add events.
    public void Subscribe(TransactionService service)
    {
        service.TransactionAdded += OnTransactionAdded;
    }

    // Writes entries safely and suppresses file system failures.
    private void OnTransactionAdded(object? sender, Events.TransactionAddedEventArgs e)
    {
        try
        {
            var transaction = e.Transaction;

            string sign = transaction.Type == Models.TransactionType.Income ? "+" : "-";

            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                           $"{transaction.Type} | {sign}{transaction.Amount:F2} | " +
                           $"{transaction.Description}";

            File.AppendAllText(_logFilePath, entry + Environment.NewLine);
        }
        catch
        {
            // Logging errors are intentionally ignored.
        }
    }
}
