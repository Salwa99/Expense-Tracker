using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("totalExpenses")]
        public decimal TotalExpenses { get; set; }

        [JsonPropertyName("categoryExpenses")]
        public List<CategoryExpense> CategoryExpenses { get; set; }

        [JsonPropertyName("monthlyExpenses")]
        public List<MonthlyExpense> MonthlyExpenses { get; set; }

        [JsonPropertyName("recentExpenses")]
        public List<Expense> RecentExpenses { get; set; }
    }

    public class CategoryExpense
    {
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }
    }

    public class MonthlyExpense
    {
        [JsonPropertyName("month")]
        public string Month { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }

    public class ExpenseFilterViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Sort By")]
        public string SortOrder { get; set; }

        // Helper property for sort order options
        public static class SortOrders
        {
            public const string DateDesc = "date_desc";
            public const string DateAsc = "date_asc";
            public const string AmountDesc = "amount_desc";
            public const string AmountAsc = "amount_asc";
        }
    }
}