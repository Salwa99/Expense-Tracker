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

            // Get all user expenses
            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == user.Id)
                .ToListAsync();

            // Calculate total expenses
            var totalExpenses = expenses.Sum(e => e.Amount);

            // Calculate expenses by category
            var categoryExpenses = expenses
                .GroupBy(e => e.Category.Name)
                .Select(g => new CategoryExpense
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(e => e.Amount),
                    Color = GetRandomColor() // Helper method to generate colors
                })
                .ToList();

            // Calculate monthly expenses
            var monthlyExpenses = expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new MonthlyExpense
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Amount = g.Sum(e => e.Amount)
                })
                .OrderBy(m => m.Month)
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