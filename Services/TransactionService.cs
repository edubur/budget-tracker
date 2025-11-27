using BudgetTracker.Events;
using BudgetTracker.Models;

namespace BudgetTracker.Services;

// Provides business logic for adding, removing,
// and querying transactions.
// Interacts with StorageService but does not perform file I/O directly.
public class TransactionService
{
    private readonly StorageService _storageService;

    // Event raised whenever a transaction is successfully added.
    public event EventHandler<TransactionAddedEventArgs>? TransactionAdded;

    // Constructor accepts a StorageService dependency.
    public TransactionService(StorageService storageService)
    {
        _storageService = storageService;
    }

    // Adds a transaction after assigning required metadata.
    // Raises the TransactionAdded event only after persistence succeeds.
    public void Add(Transaction transaction)
    {
        ValidateTransaction(transaction);

        _storageService.AppendTransaction(transaction);

        OnTransactionAdded(transaction);
    }

    // Attempts to remove a transaction by ID and date.
    // Returns true if removal succeeds; otherwise false.
    public bool Remove(Guid id, DateTime date)
    {
        return _storageService.DeleteTransaction(id, date);
    }

    // Retrieves transactions within the given date range.
    // Combines data from multiple files into one collection.
    public IEnumerable<Transaction> GetByDateRange(DateTime start, DateTime end)
    {
        var files = _storageService.GetFilesInRange(start, end);

        foreach (var file in files)
        {
            foreach (var transaction in _storageService.ReadFromFile(file))
            {
                yield return transaction;
            }
        }
    }

    // Validates transaction fields before saving.
    private void ValidateTransaction(Transaction transaction)
    {
        if (transaction.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (!Enum.IsDefined(typeof(TransactionType), transaction.Type))
            throw new ArgumentException("Invalid transaction type.");

        if (string.IsNullOrWhiteSpace(transaction.Description))
            throw new ArgumentException("Description cannot be empty.");
    }

    // Raises the TransactionAdded event safely.
    protected virtual void OnTransactionAdded(Transaction transaction)
    {
        TransactionAdded?.Invoke(this, new TransactionAddedEventArgs(transaction));
    }
}
