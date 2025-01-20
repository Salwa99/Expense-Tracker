using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Expense_Tracker.Models.ViewModels;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get all user expenses
            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == user.Id)
                .ToListAsync();

            // Calculate total expenses
            var totalExpenses = expenses.Sum(e => e.Amount);

            // Calculate expenses by category with fixed colors
            var categoryColors = new Dictionary<string, string>
    {
        {"Housing", "#FF6384"},
        {"Transportation", "#36A2EB"},
        {"Food", "#FFCE56"},
        {"Utilities", "#4BC0C0"},
        {"Insurance", "#9966FF"},
        {"Healthcare", "#FF9F40"},
        {"Entertainment", "#EA80FC"},
        {"Shopping", "#607D8B"},
        {"Education", "#00E676"},
        {"Other", "#FF5722"}
    };

            var categoryExpenses = expenses
                .GroupBy(e => e.Category.Name)
                .Select(g => new CategoryExpense
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(e => e.Amount),
                    Color = categoryColors.ContainsKey(g.Key) ? categoryColors[g.Key] : "#808080"
                })
                .ToList();

            // Calculate monthly expenses for the past 6 months
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(d => d)
                .ToList();

            var monthlyExpenses = last6Months
                .GroupJoin(
                    expenses,
                    month => new { Year = month.Year, Month = month.Month },
                    expense => new { Year = expense.Date.Year, Month = expense.Date.Month },
                    (month, expenseGroup) => new MonthlyExpense
                    {
                        Month = month.ToString("MMM yyyy"),
                        Amount = expenseGroup.Sum(e => e.Amount)
                    })
                .ToList();

            // Get recent expenses
            var recentExpenses = expenses
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TotalExpenses = totalExpenses,
                CategoryExpenses = categoryExpenses,
                MonthlyExpenses = monthlyExpenses,
                RecentExpenses = recentExpenses
            };

            return View(viewModel);
        }

        private string GetRandomColor()
        {
            var colors = new[]
            {
                "#FF6384",
                "#36A2EB",
                "#FFCE56",
                "#4BC0C0",
                "#9966FF",
                "#FF9F40"
            };

            Random rand = new Random();
            return colors[rand.Next(colors.Length)];
        }
    }
}