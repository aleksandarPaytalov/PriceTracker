# 🚀 Database Seeding Instructions

## 📍 JSON Files Location
Place your JSON files in:

PriceTracker.Infrastructure/
└── Data/
    └── SeedDatabase/
        └── JsonData/
            ├── products.json
            ├── stores.json
            ├── prices.json
            ├── expenses.json
            ├── tasks.json
            ├── notifications.json
            ├── budgets.json
            ├── roles.json
            ├── users.json
            └── userroles.json

----------------------------------------------------------------

## 🎯 OPTION 1: DEFAULT Data (Recommended for first test)

### 1. Configure appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(yourServer);Database=(databaseName);Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "SeedingOptions": {
    "UseExternalSource": false,
    "StrictValidation": true
  }
}

### 2. Run Migration Commands:

# Create migration
Add-Migration InitialSeedDefault

# Apply migration
Update-Database

### 3. Expected Results:
- ✅ 3 Roles (Administrator, User, Guest)
- ✅ 3 Users with hashed passwords
- ✅ 8 Stores
- ✅ 9 Products
- ✅ 9 Prices
- ✅ 6 Expenses
- ✅ 3 Tasks
- ✅ 3 Notifications
- ✅ 3 Monthly Budgets

---------------------------------------------------------------

## 🎯 OPTION 2: JSON Data (For real data)

### 1. Prepare JSON Files:

// ✅ KEEP ONLY VALID JSON:
[
  {
    "expenseId": 1,
    "userId": "56f4b198-3f3e-4f72-9a80-7d903bf24a1e",
    "expenseType": 2,
    "amountSpent": 31.59
  }
]

### 2. Configure appsettings.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(yourServer);Database=(databaseName);Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "SeedingOptions": {
    "UseExternalSource": true,
    "DataFolderPath": "Data/SeedDatabase/JsonData",
    "StrictValidation": true,
    "EnabledSeeders": {
      "Product": true,
      "Store": true,
      "Price": true,
      "Expense": true,
      "Task": true,
      "Notification": true,
      "Budget": true,
      "Role": true,
      "User": true,
      "UserRole": true
    }
  }
}
```

### 3. Run Migration Commands:

# Create migration
Add-Migration InitialSeedJson

# Apply migration
Update-Database

### 4. Expected Results:
- ✅ 2 Roles from JSON
- ✅ 1 User from JSON  
- ✅ 20 Stores from JSON
- ✅ 50 Products from JSON
- ✅ 70 Prices from JSON
- ✅ 10 Expenses from JSON
- ✅ 10 Tasks from JSON
- ✅ 10 Notifications from JSON
- ✅ 10 Monthly Budgets from JSON

--------------------------------------------------------------------------------

## ⚙️ StrictValidation Settings

### `"StrictValidation": true` (Recommended for Development)
- **Behavior:** Stops migration on first validation error
- **Use when:** 
  - Development and testing
  - First time setup
  - Want to catch all data issues
- **Result:** Migration fails if any record is invalid

### `"StrictValidation": false` (Recommended for Production)
- **Behavior:** Skips invalid records, continues with valid ones
- **Use when:**
  - Production environment
  - Large JSON files with possible errors
  - Want to import as much valid data as possible
- **Result:** Invalid records are logged and skipped

--------------------------------------------------------------------------------

## 🎯 Recommended Workflow

1. **Start with DEFAULT data** - Test that everything works
2. **Clean your JSON files** - Remove all comments
3. **Switch to JSON data** - Change UseExternalSource to true
4. **Use StrictValidation: true** - During development
5. **Switch to StrictValidation: false** - For production

--------------------------------------------------------------------------------

**🎉 Success Indicators:**
- ✅ Migration created without errors
- ✅ Database update completed successfully  
- ✅ Logs show successful seeding
- ✅ SQL query shows expected record counts
- ✅ No ERROR entries in logs