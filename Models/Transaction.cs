using System;

namespace BudgetTracker.Models;

// This class represents a single financial transaction in the system.
// I designed it to be a simple data model without any business logic or file handling.
// Its only job is to hold information about one income or expense entry.

public class Transaction
{
    // A unique identifier for each transaction.
    // This allows transactions to be removed reliably even if multiple have the same description or amount.
    public Guid Id { get; set; }

    // The exact date and time when the transaction was created.
    // Mainly for logging and auditing purposes.
    public DateTime Timestamp { get; set; }

    // The calendar date of the transaction (without time).
    // As the key for daily JSON file storage (e.g. 2025-11-26.json).
    public DateTime Date { get; set; }

    // Indicates whether the transaction is an Income or an Expense.
    // Using an enum here avoids invalid string values and improves safety.
    public TransactionType Type { get; set; }

    // A short description explaining the transaction.
    // This helps users understand what each entry represents.
    public string Description { get; set; } = string.Empty;

    // The monetary value of the transaction.
    // Positive numbers only — whether it's income or expense is determined by the Type.
    public decimal Amount { get; set; }
}
