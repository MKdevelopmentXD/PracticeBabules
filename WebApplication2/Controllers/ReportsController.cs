using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ExpensesDbContext _context;

        public ReportsController(ExpensesDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var reportTypes = new List<SelectListItem>
        {
            new SelectListItem { Value = "category", Text = "Expenses by Category" },
            new SelectListItem { Value = "daybyday", Text = "Day by Day Expenses" }
        };

            ViewBag.ReportTypes = reportTypes;

            return View();
        }

        [HttpPost]
        public IActionResult Reports(string reportType, DateTime? month)
        {
            var startDate = DateTime.Now.Date.AddDays(-30);
            switch (reportType)
            {
                case "expensesbycategory":
                    return ExpensesByCategoryReport(startDate);
                case "revenuesbycategory":
                    //return RevenuesByCategoryReport(startDate);
                default:
                    return BadRequest("Selected report type is not supported.");
            }

            if (string.IsNullOrEmpty(reportType))
            {
                return RedirectToAction(nameof(Reports), new { reportType = "category" });
            }

            if (reportType == "category")
            {
                if (!month.HasValue)
                {
                    month = DateTime.Now;
                }

                var transactions = _context.Transactions
                    .Where(t => t.TransactionDate.Month == month.Value.Month && t.TransactionDate.Year == month.Value.Year)
                    .Include(t => t.Category)
                    .ToList();

                var expenses = transactions
                    .Where(t => t.Type == "Expense")
                    .GroupBy(t => t.Category)
                    .Select(g => new CategoryViewModel
                    {
                        CategoryName = g.Key.Name,
                        TotalAmount = g.Sum(t => t.Amount)
                    })
                    .ToList();

                var revenues = transactions
                    .Where(t => t.Type == "Revenue")
                    .GroupBy(t => t.Category)
                    .Select(g => new CategoryViewModel
                    {
                        CategoryName = g.Key.Name,
                        TotalAmount = g.Sum(t => t.Amount)
                    })
                    .ToList();

                var viewModel = new ReportsViewModel
                {
                    ReportType = reportType,
                    Month = month.Value,
                    Expenses = expenses,
                    Revenues = revenues
                };

                return View();
            }

           
        }

        private IActionResult ExpensesByCategoryReport(DateTime? startDate)
        {
            startDate ??= DateTime.Now.Date.AddDays(-30); // default to last 30 days if start date is not specified
            var expensesByCategory = _context.Transactions
                .Where(t => t.Type == "Expense" && t.TransactionDate >= startDate.Value)
                .GroupBy(t => t.Category.Name)
                .Select(g => new CategoryViewModel
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .ToList();

            ViewData["StartDate"] = startDate.Value.ToShortDateString();
            return View("ExpensesByCategoryReport", expensesByCategory);
        }

        /*public IActionResult ExpensesByCategory()
        {
            var expenses = _context.Transactions
                .Where(t => t.Type == "Expense")
                .Where(t => t.TransactionDate >= DateTime.Today.AddDays(-30))
                .GroupBy(t => t.Category.Name)
                .Select(group => new CategoryViewModel
                {
                    CategoryName = group.Key,
                    TotalAmount = group.Sum(t => t.Amount)
                })
                .ToList();

            var revenues = _context.Transactions
                .Where(t => t.Type == "Revenue")
                .Where(t => t.TransactionDate >= DateTime.Today.AddDays(-30))
                .GroupBy(t => t.Category.Name)
                .Select(group => new CategoryViewModel
                {
                    CategoryName = group.Key,
                    TotalAmount = group.Sum(t => t.Amount)
                })
                .ToList();

            return View();
        }

        public IActionResult DayByDayExpenses()
        {
            var dayByDayExpenses = _context.Transactions
                .Where(t => t.TransactionDate >= DateTime.Today.AddDays(-30)) // Get transactions from the past 30 days
                .GroupBy(t => t.Date)
                .Select(group => new DateExpensesViewModel
                {
                    Date = group.Key,
                    TotalAmount = group.Sum(t => t.Amount),
                    Transactions = group.ToList()
                })
                .OrderByDescending(d => d.Date)
                .ToList();

            return View(dayByDayExpenses);
        }
        */
    }
}
