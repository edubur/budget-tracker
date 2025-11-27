namespace BudgetTracker.Services;

// Handles logging of transactions based on raised events.
// This class is fully event-driven and contains no business logic.
public class LoggerService
{
    private readonly string _logFilePath;

    // Constructor requires the log file path.
    // Ensures the directory exists before writing.
    public LoggerService(string logFilePath)
    {
        _logFilePath = logFilePath;

        var directory = Path.GetDirectoryName(_logFilePath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }

    // Subscribes to the TransactionAdded event.
    public void Subscribe(TransactionService service)
    {
        service.TransactionAdded += OnTransactionAdded;
    }

    // Handles the TransactionAdded event.
    // Writes a formatted log entry for each transaction.
    private void OnTransactionAdded(object? sender, Events.TransactionAddedEventArgs e)
    {
        var transaction = e.Transaction;

        string sign = transaction.Type == Models.TransactionType.Income ? "+" : "-";

        string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                       $"{transaction.Type} | {sign}{transaction.Amount:F2} | " +
                       $"{transaction.Description}";

        File.AppendAllText(_logFilePath, entry + Environment.NewLine);
    }
}
