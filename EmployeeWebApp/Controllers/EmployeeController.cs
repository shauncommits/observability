using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeWebApp.Models;

namespace EmployeeWebApp.Controllers;

public class EmployeeController : Controller
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly EmployeeDbContext _dbContext;

    public EmployeeController(ILogger<EmployeeController> logger, EmployeeDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IEnumerable<Employee> Index()
    {
        var response = _dbContext.Employees.ToList();
        return response;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

