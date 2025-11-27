namespace BudgetTracker.Models;

// This enum strictly limits the type of transactions
// to only two allowed values: Income and Expense.
// This prevents invalid types and makes filtering/reporting much easier.

public enum TransactionType
{
    Income,
    Expense
}
