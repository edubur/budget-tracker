using BudgetTracker.Models;
using BudgetTracker.Services;

string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
string logsDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
string logFile = Path.Combine(logsDir, "transactions.log");

// Ensures required directories exist before application execution.
if (!Directory.Exists(dataDir))
    Directory.CreateDirectory(dataDir);

if (!Directory.Exists(logsDir))
    Directory.CreateDirectory(logsDir);

// Initializes services.
var storageService = new StorageService(dataDir);
var transactionService = new TransactionService(storageService);

// Configures logging through event subscription.
var logger = new LoggerService(logFile);
logger.Subscribe(transactionService);

// Application loop.
while (true)
{
    Console.WriteLine("\n=== BUDGET TRACKER ===");
    Console.WriteLine("1. Add Transaction");
    Console.WriteLine("2. Remove Transaction");
    Console.WriteLine("3. Generate Report");
    Console.WriteLine("4. Exit");
    Console.Write("Select an option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddTransaction(transactionService);
            break;

        case "2":
            RemoveTransaction(transactionService);
            break;

        case "3":
            GenerateReport(transactionService);
            break;

        case "4":
            Console.WriteLine("Exiting application...");
            return;

        default:
            Console.WriteLine("Invalid selection.");
            break;
    }
}

// Prompts user for transaction details and submits it to the service.
static void AddTransaction(TransactionService service)
{
    Console.Write("Enter type (Income/Expense): ");
    if (!Enum.TryParse(Console.ReadLine(), true, out TransactionType type))
    {
        Console.WriteLine("Invalid type.");
        return;
    }

    Console.Write("Enter description: ");
    string? description = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(description))
    {
        Console.WriteLine("Description is required.");
        return;
    }

    Console.Write("Enter amount: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
    {
        Console.WriteLine("Invalid amount.");
        return;
    }

    var transaction = new Transaction
    {
        Id = Guid.NewGuid(),
        Timestamp = DateTime.Now,
        Date = DateTime.Today,
        Type = type,
        Description = description,
        Amount = amount
    };

    try
    {
        service.Add(transaction);
        Console.WriteLine("Transaction added successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// Prompts user for ID and date to remove a transaction.
static void RemoveTransaction(TransactionService service)
{
    Console.Write("Enter transaction ID: ");
    if (!Guid.TryParse(Console.ReadLine(), out Guid id))
    {
        Console.WriteLine("Invalid ID format.");
        return;
    }

    Console.Write("Enter transaction date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
    {
        Console.WriteLine("Invalid date.");
        return;
    }

    bool removed = service.Remove(id, date.Date);

    if (removed)
        Console.WriteLine("Transaction removed.");
    else
        Console.WriteLine("Transaction not found.");
}

// Generates a simple summary report based on a date range.
static void GenerateReport(TransactionService service)
{
    Console.Write("Enter start date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime start))
    {
        Console.WriteLine("Invalid start date.");
        return;
    }

    Console.Write("Enter end date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime end))
    {
        Console.WriteLine("Invalid end date.");
        return;
    }

    var transactions = service.GetByDateRange(start.Date, end.Date).ToList();

    if (!transactions.Any())
    {
        Console.WriteLine("No transactions found.");
        return;
    }

    decimal income = transactions
        .Where(t => t.Type == TransactionType.Income)
        .Sum(t => t.Amount);

    decimal expenses = transactions
        .Where(t => t.Type == TransactionType.Expense)
        .Sum(t => t.Amount);

    Console.WriteLine("\n=== REPORT ===");
    Console.WriteLine($"Total Income:   {income:F2}");
    Console.WriteLine($"Total Expenses: {expenses:F2}");
    Console.WriteLine($"Net Balance:    {income - expenses:F2}");
}
