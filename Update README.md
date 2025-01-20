# 💰 Expense Tracker

## 📝 Description

A comprehensive web application built with ASP.NET Core MVC that helps users track and manage their personal expenses. The application features a clean, modern interface built with Tailwind CSS and includes interactive charts for expense visualization.

## ✨ Features

- **User Authentication**
  - Secure registration and login
  - Password recovery
  - User profile management

- **Expense Management**
  - Add, edit, and delete expenses
  - Categorize expenses
  - Add descriptions and dates

- **Dashboard Analytics**
  - Visual expense breakdown
  - Category-wise analytics
  - Monthly trends
  - Recent transactions list

- **Categories**
  - Predefined expense categories
  - Housing
  - Transportation
  - Food & Dining
  - Utilities
  - Healthcare
  - Entertainment
  - And more...

## 🛠 Built With

- ASP.NET Core MVC 8.0.10
- Entity Framework Core
- SQL Server
- Identity Framework
- Tailwind CSS
- Chart.js

## 💻 Prerequisites

Before you begin, ensure you have met the following requirements:
- .NET SDK 8.0.10 or later
- SQL Server (LocalDB or full version)
- Node.js and npm (for Tailwind CSS)
- Visual Studio 2022 or VS Code

## 🚀 Installation

1. Clone the repository
```bash
git clone https://github.com/Salwa99/Expense-Tracker.git
cd Expense-Tracker
```

2. Update the connection string in appsettings.json:
```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ExpenseTracker;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```
3. Install dependencies:
```bash
# Install .NET dependencies
dotnet restore

# Install npm packages
npm install
```
4. Apply database migrations:
```bash
dotnet ef database update
```
5. Run the application:
```bash
dotnet run
```


## 📱 Usage
1. Register a new account
2. Log in to your account
3. Add your expenses with categories
4. View your expense breakdown on the dashboard
5. Track your spending patterns through visualizations
6. Manage your expenses through the expense list

## 👥 Authors
### 👤 Salwa Ballouti

GitHub: @Salwa99
LinkedIn: Salwa Ballouti

## 🤝 Contributing
Contributions, issues, and feature requests are welcome!
Feel free to check the issues page.

## ⭐️ Show your support
Give a ⭐️ if you like this project!

## 📝 License
This project is MIT licensed.

