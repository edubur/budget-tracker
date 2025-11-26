using BudgetTracker.Models;

namespace BudgetTracker.Events;

// Custom event arguments for transaction notifications.
public class TransactionAddedEventArgs : EventArgs
{
    // Exposes the newly added transaction.
    public Transaction Transaction { get; }

    public TransactionAddedEventArgs(Transaction transaction)
    {
        Transaction = transaction;
    }
}
