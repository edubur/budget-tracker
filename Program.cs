using BudgetTracker.Models;
using BudgetTracker.Services;

string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
string logsDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
string logFile = Path.Combine(logsDir, "transactions.log");

// Ensures required folders exist before services are created.
Directory.CreateDirectory(dataDir);
Directory.CreateDirectory(logsDir);

// Service initialization.
var storageService = new StorageService(dataDir);
var transactionService = new TransactionService(storageService);

// Logging service wired to event system.
var logger = new LoggerService(logFile);
logger.Subscribe(transactionService);

// Menu loop.
while (true)
{
    Console.Clear();
    Console.WriteLine("=== BUDGET TRACKER ===\n");
    Console.WriteLine("1. Add Transaction");
    Console.WriteLine("2. Remove Transaction");
    Console.WriteLine("3. Generate Report");
    Console.WriteLine("4. Exit");
    Console.Write("\nSelect option: ");

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
            if (Confirm("Exit application?"))
                return;
            break;
        default:
            Console.WriteLine("Invalid selection.");
            Pause();
            break;
    }
}

// Adds a new transaction with validation.
static void AddTransaction(TransactionService service)
{
    Console.Clear();
    Console.WriteLine("--- ADD TRANSACTION ---");

    Console.Write("Type (Income/Expense): ");
    if (!Enum.TryParse(Console.ReadLine(), true, out TransactionType type))
    {
        Console.WriteLine("Invalid type.");
        Pause();
        return;
    }

    Console.Write("Description: ");
    string? description = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(description))
    {
        Console.WriteLine("Description required.");
        Pause();
        return;
    }

    Console.Write("Amount: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
    {
        Console.WriteLine("Invalid amount.");
        Pause();
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

    Pause();
}

// Removes a transaction by ID and confirms deletion.
static void RemoveTransaction(TransactionService service)
{
    Console.Clear();
    Console.WriteLine("--- REMOVE TRANSACTION ---");

    Console.Write("Date of transaction (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
    {
        Console.WriteLine("Invalid date.");
        Pause();
        return;
    }

    var transactions = service.GetByDateRange(date.Date, date.Date).ToList();

    if (!transactions.Any())
    {
        Console.WriteLine("No entries found for this date.");
        Pause();
        return;
    }

    Console.WriteLine("\nAvailable transactions:");
    foreach (var t in transactions)
    {
        Console.WriteLine($"{t.Id} | {t.Type} | {t.Amount:F2} | {t.Description}");
    }

    Console.Write("\nEnter ID: ");
    if (!Guid.TryParse(Console.ReadLine(), out Guid id))
    {
        Console.WriteLine("Invalid ID.");
        Pause();
        return;
    }

    if (!Confirm("Confirm deletion?"))
        return;

    bool removed = service.Remove(id, date.Date);

    Console.WriteLine(removed ? "Deleted." : "Transaction not found.");
    Pause();
}

// Generates grouped reports with totals.
static void GenerateReport(TransactionService service)
{
    Console.Clear();
    Console.WriteLine("--- REPORT ---");

    Console.Write("Start date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime start))
    {
        Console.WriteLine("Invalid start date.");
        Pause();
        return;
    }

    Console.Write("End date (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out DateTime end))
    {
        Console.WriteLine("Invalid end date.");
        Pause();
        return;
    }

    if (start > end)
    {
        Console.WriteLine("Start date must not exceed end date.");
        Pause();
        return;
    }

    var results = service.GetByDateRange(start.Date, end.Date).ToList();

    if (!results.Any())
    {
        Console.WriteLine("No data in range.");
        Pause();
        return;
    }

    var dailyGroups = results
        .OrderBy(t => t.Date)
        .GroupBy(t => t.Date);

    decimal totalIncome = 0;
    decimal totalExpense = 0;

    foreach (var day in dailyGroups)
    {
        var income = day
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var expense = day
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        Console.WriteLine($"\n== {day.Key:yyyy-MM-dd} ==");
        Console.WriteLine($"Income:  {income:F2}");
        Console.WriteLine($"Expense: {expense:F2}");
        Console.WriteLine($"Net:     {(income - expense):F2}");

        totalIncome += income;
        totalExpense += expense;
    }

    Console.WriteLine("\n--- GRAND TOTALS ---");
    Console.WriteLine($"Income:  {totalIncome:F2}");
    Console.WriteLine($"Expense: {totalExpense:F2}");
    Console.WriteLine($"NET:     {(totalIncome - totalExpense):F2}");

    Pause();
}

// Displays confirmation prompt.
static bool Confirm(string message)
{
    Console.Write($"{message} (y/n): ");
    return Console.ReadLine()?.Trim().ToLower() == "y";
}

// Suspends execution until key press.
static void Pause()
{
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}
