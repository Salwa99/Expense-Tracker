using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class DashboardViewModel
    {
        public decimal TotalExpenses { get; set; }
        public List<CategoryExpense> CategoryExpenses { get; set; }
        public List<MonthlyExpense> MonthlyExpenses { get; set; }
        public List<Expense> RecentExpenses { get; set; }
    }

    public class CategoryExpense
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Color { get; set; }
    }

    public class MonthlyExpense
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }
}