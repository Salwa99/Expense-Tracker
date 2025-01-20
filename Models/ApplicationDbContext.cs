using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;

namespace Expense_Tracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // relationships
            builder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); ;

            builder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); ;

            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Housing", Description = "Rent, Mortgage, Repairs" },
                new Category { Id = 2, Name = "Transportation", Description = "Car payment, Gas, Public transit" },
                new Category { Id = 3, Name = "Food", Description = "Groceries, Dining out" },
                new Category { Id = 4, Name = "Utilities", Description = "Electric, Water, Phone, Internet" },
                new Category { Id = 5, Name = "Insurance", Description = "Health, Car, Home insurance" },
                new Category { Id = 6, Name = "Healthcare", Description = "Medical, Dental, Vision" },
                new Category { Id = 7, Name = "Entertainment", Description = "Movies, Games, Hobbies" },
                new Category { Id = 8, Name = "Shopping", Description = "Clothing, Electronics" },
                new Category { Id = 9, Name = "Education", Description = "Tuition, Books, Courses" },
                new Category { Id = 10, Name = "Other", Description = "Miscellaneous expenses" }
            );
        }
    }
}