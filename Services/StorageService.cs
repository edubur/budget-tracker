using System.Text.Json;
using BudgetTracker.Models;

namespace BudgetTracker.Services;

// Handles all file system and JSON responsibilities.
// No business logic or validation exists in this class.
public class StorageService
{
    private readonly string _dataDirectory;

    // Constructor accepts the data directory path.
    // Ensures the directory exists on startup.
    public StorageService(string dataDirectory)
    {
        _dataDirectory = dataDirectory;

        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }
    }

    // Builds the absolute file path for a given date.
    // File format: yyyy-MM-dd.json
    private string GetFilePath(DateTime date)
    {
        string fileName = $"{date:yyyy-MM-dd}.json";
        return Path.Combine(_dataDirectory, fileName);
    }

    // Loads all transactions for the given day.
    // Returns an empty list if the file does not exist.
    public List<Transaction> ReadByDate(DateTime date)
    {
        string filePath = GetFilePath(date);

        if (!File.Exists(filePath))
            return new List<Transaction>();

        string json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Transaction>();

        var result = JsonSerializer.Deserialize<List<Transaction>>(json);

        return result ?? new List<Transaction>();
    }

    // Writes a full list of transactions to the specified day's file.
    // Overwrites the entire file to maintain data consistency.
    public void WriteByDate(DateTime date, List<Transaction> transactions)
    {
        string filePath = GetFilePath(date);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(transactions, options);
        File.WriteAllText(filePath, json);
    }

    // Appends a single transaction into the correct daily file.
    public void AppendTransaction(Transaction transaction)
    {
        var existing = ReadByDate(transaction.Date);
        existing.Add(transaction);
        WriteByDate(transaction.Date, existing);
    }

    // Deletes a transaction by ID from its file.
    // Returns true when deletion succeeds.
    // Returns false if the transaction is not found.
    public bool DeleteTransaction(Guid id, DateTime date)
    {
        var transactions = ReadByDate(date);

        var target = transactions.FirstOrDefault(t => t.Id == id);

        if (target == null)
            return false;

        transactions.Remove(target);
        WriteByDate(date, transactions);

        return true;
    }

    // Retrieves all JSON files that fall within a date range.
    public IEnumerable<string> GetFilesInRange(DateTime start, DateTime end)
    {
        var files = Directory.GetFiles(_dataDirectory, "*.json");

        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            if (DateTime.TryParse(fileName, out var fileDate))
            {
                if (fileDate >= start && fileDate <= end)
                {
                    yield return file;
                }
            }
        }
    }

    // Reads transactions from a given JSON file path.
    public List<Transaction> ReadFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
    }
}
