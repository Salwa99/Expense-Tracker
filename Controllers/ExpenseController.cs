using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Data;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpenseController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == user.Id)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            return View(expenses);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                // Get current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Create new expense
                var newExpense = new Expense
                {
                    Description = expense.Description,
                    Amount = expense.Amount,
                    Date = expense.Date,
                    CategoryId = expense.CategoryId,
                    UserId = user.Id
                };

                // Add and save
                _context.Expenses.Add(newExpense);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
            }

            // If ModelState is invalid, return to form
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var existingExpense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (existingExpense == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingExpense.Description = expense.Description;
                    existingExpense.Amount = expense.Amount;
                    existingExpense.Date = expense.Date.Date;
                    existingExpense.CategoryId = expense.CategoryId;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(expense);
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}