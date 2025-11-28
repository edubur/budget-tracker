# BudgetTracker

A lightweight, console-based C# application for tracking personal income and expenses. Data is stored as daily JSON files, and each added transaction is logged through an event-driven logging system. Ideal for learning clean separation of concerns (models, services, events) and basic persistence without a database.

## Features

- Add income or expense transactions with validation.
- Remove transactions by GUID and date.
- Generate summarized reports over a date range (daily income, expense, net, grand totals).
- JSON-based, file-per-day storage (e.g. `2025-11-27.json`).
- Event-driven logging of additions (`logs/transactions.log`).
- Clean architecture: `Models`, `Services`, `Events`.

## Project Structure

```
BudgetTracker/
├── Program.cs                # Menu + user interaction
├── Models/                   # Data structures (Transaction, TransactionType)
├── Services/                 # Storage, business logic, logging
├── Events/                   # Custom event args for transaction notifications
├── data/                     # Generated JSON files per day (created at runtime)
├── logs/                     # Transaction log file (created at runtime)
```

## Requirements

- .NET SDK 8 or later (project targets `net10.0`; ensure you have a compatible preview/runtime).
- macOS/Linux/Windows terminal.

Check your installed SDKs:

```bash
dotnet --list-sdks
```

If `10.0.x` is not present, install the latest .NET preview from: https://dotnet.microsoft.com/download/dotnet

## Getting Started

Clone the repository:

```bash
git clone https://github.com/edubur/budget-tracker.git
cd budget-tracker
```

Restore (not strictly needed for this simple project, but good practice):

```bash
dotnet restore
```

Run the app:

```bash
dotnet run
```

## Usage

1. Choose an option from the menu:
   - `Add Transaction`: Enter type (`Income` or `Expense`), description, and positive amount.
   - `Remove Transaction`: Provide a date, list matching entries, then enter the GUID to delete.
   - `Generate Report`: Enter start and end dates to see per-day and grand totals.
   - `Exit`: Quit the application.
2. Added transactions are persisted into `data/YYYY-MM-DD.json`.
3. Each successful addition is logged to `logs/transactions.log`.

### Example Log Entry

```
[2025-11-27 12:34:56] Income | +250.00 | Freelance payment
```

## Data Format (JSON)

Each daily file contains an array of transactions:

```json
[
  {
    "Id": "d9f8e4b2-6c0d-4cd9-9c40-123456789abc",
    "Timestamp": "2025-11-27T12:34:56.789Z",
    "Date": "2025-11-27T00:00:00",
    "Type": 0, // 0 = Income, 1 = Expense
    "Description": "Freelance payment",
    "Amount": 250.0
  }
]
```

## Design Highlights

- Separation of concerns:
  - `StorageService`: Pure persistence (JSON + file paths).
  - `TransactionService`: Business rules + event raising.
  - `LoggerService`: Subscribes to `TransactionService` events.
- Event-driven architecture prevents logging code from polluting business logic.
- GUID identifiers ensure safe deletion even for duplicate descriptions/amounts.

## Error Handling

- Input validation prevents invalid amounts, empty descriptions, wrong types.
- Logging failures are swallowed to avoid user disruption.
- Safe defaults: missing or empty JSON files yield an empty list.

## Extending the App

Ideas for future improvement:

- Add category tagging (e.g., Food, Rent, Salary).
- Monthly/annual roll-up reports.
- Export CSV or HTML report.
- Support editing existing transactions.
- Add unit tests (e.g., for `TransactionService` validation and reporting).
- Introduce a persistence abstraction for easier future migration to a database.

## Running Tests (Future)

If tests are added later:

```bash
dotnet test
```

## Contributing

1. Create a feature branch: `git checkout -b feature/my-change`
2. Commit with clear messages.
3. Open a Pull Request.

## License

Currently unspecified. Add a license (e.g., MIT) if you plan to share/distribute.

---

Happy tracking! Feel free to open issues or propose enhancements.
