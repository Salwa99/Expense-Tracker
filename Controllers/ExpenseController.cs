using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Data;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Expense_Tracker.Models.ViewModels;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ExpenseController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? categoryId, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == user.Id);

            // category filter
            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            // date filter
            if (startDate.HasValue)
            {
                query = query.Where(e => e.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(e => e.Date <= endDate.Value);
            }

            // sorting
            query = sortOrder switch
            {
                "date_asc" => query.OrderBy(e => e.Date),
                "date_desc" => query.OrderByDescending(e => e.Date),
                "amount_asc" => query.OrderBy(e => e.Amount),
                "amount_desc" => query.OrderByDescending(e => e.Amount),
                _ => query.OrderByDescending(e => e.Date) // Default sort
            };

            var expenses = await query.ToListAsync();

            var viewModel = new ExpenseFilterViewModel
            {
                Expenses = expenses,
                CategoryId = categoryId,
                SortOrder = sortOrder,
                StartDate = startDate,
                EndDate = endDate
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", categoryId);

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(new Expense { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Amount,Date,CategoryId")] Expense expense)
        {
            try
            {
                // Get current user first
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Remove validation for UserId, User, and Category navigation property
                ModelState.Remove("UserId");
                ModelState.Remove("User");
                ModelState.Remove("Category");

                if (ModelState.IsValid)
                {
                    // Validate CategoryId exists
                    if (!_context.Categories.Any(c => c.Id == expense.CategoryId))
                    {
                        ModelState.AddModelError("CategoryId", "Please select a valid category");
                        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", expense.CategoryId);
                        return View(expense);
                    }

                    // Set the user ID before saving
                    expense.UserId = user.Id;

                    _context.Expenses.Add(expense);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Expense created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving expense: " + ex.Message);
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", expense.CategoryId);
            return View(expense);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var expense = await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (expense == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(
                await _context.Categories.ToListAsync(),
                "Id",
                "Name",
                expense.CategoryId
            );

            return View(expense);
        }

        [HttpPut("Expense/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] Expense expense)
        {
            try
            {
                if (id != expense.Id)
                {
                    return BadRequest(new { success = false, message = "Invalid expense ID" });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                // Verify expense exists and belongs to current user
                var existingExpense = await _context.Expenses
                    .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

                if (existingExpense == null)
                {
                    return NotFound(new { success = false, message = "Expense not found" });
                }

                ModelState.Remove("Category");
                ModelState.Remove("User");
                ModelState.Remove("UserId"); 

                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == expense.CategoryId);
                if (!categoryExists)
                {
                    return BadRequest(new { success = false, message = "Selected category does not exist" });
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Update properties
                        existingExpense.Description = expense.Description;
                        existingExpense.Amount = expense.Amount;
                        existingExpense.Date = expense.Date;
                        existingExpense.CategoryId = expense.CategoryId;
                        existingExpense.UserId = user.Id;

                        _context.Entry(existingExpense).State = EntityState.Modified;

                        await _context.SaveChangesAsync();

                        return Ok(new
                        {
                            success = true,
                            message = "Expense updated successfully",
                            data = new
                            {
                                existingExpense.Id,
                                existingExpense.Description,
                                existingExpense.Amount,
                                existingExpense.Date,
                                existingExpense.CategoryId,
                                existingExpense.UserId
                            }
                        });
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await _context.Expenses.AnyAsync(e => e.Id == id))
                        {
                            return NotFound(new { success = false, message = "Expense no longer exists" });
                        }
                        throw;
                    }
                }

                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense {ExpenseId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while updating the expense",
                    error = ex.Message
                });
            }
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