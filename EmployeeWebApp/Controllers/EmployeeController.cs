using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeWebApp.Models;
using Nest;

namespace EmployeeWebApp.Controllers;

public class EmployeeController : Controller
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly IEmployeeFactory _employeeFactory;
    private readonly IElasticClient _client;

    public EmployeeController(ILogger<EmployeeController> logger, IEmployeeFactory employeeFactory, IElasticClient client)
    {
        _logger = logger;
        _employeeFactory = employeeFactory;
        _client = client;
    }

    public async Task<IActionResult> Index(string searchQuery)
    {
        IEnumerable<Employee> employees;
            
        if (!string.IsNullOrEmpty(searchQuery))
        {
            switch (searchQuery.ToLower())
            {
                case "none":
                    searchQuery = "0";
                    break;
                case "hr":
                    searchQuery = "1";
                    break;
                case "it":
                    searchQuery = "2";
                    break;
                case "dev":
                    searchQuery = "3";
                    break;
                case "sdet":
                    searchQuery = "4";
                    break;
                case "sd1":
                    searchQuery = "5";
                    break;
                case "sd2":
                    searchQuery = "6";
                    break;
                case "architect":
                    searchQuery = "7";
                    break;
            }
            var searchResponse = _client.Search<Employee>(s => s
                .Query(q => q
                    .QueryString(qs => qs
                        .Query(searchQuery)
                    )
                )
            );

            employees = searchResponse.Documents;
        }
        else
        {
            // Retrieve all employees if no search query is provided
            
            ///// Uncomment this to show how is slow to retrieve data from a database compared using elastic search and also remove async Task<IActionResult> to just IActionResult as return type 
            // employees = _employeeFactory.GetEmployeeList();
            
            var searchResponse = await _client.SearchAsync<Employee>(s => s
                .MatchAll()
                .Index("employee_elastic_search").Size(10000));

            employees = searchResponse.Documents;
        }

        return View(employees);
    }

    public IActionResult Add()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Add(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _employeeFactory.AddEmployee(employee);
            return RedirectToAction("Index");
        }
        return View(employee);
    }
    
    public IActionResult Edit(int id)
    {
        var employee = _employeeFactory.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }
    
    [HttpPost]
    public IActionResult Edit(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _employeeFactory.UpdateEmployee(employee);
            return RedirectToAction("Index");
        }
        return View(employee);
    }
    
    public IActionResult Delete(int id)
    {
        var employee = _employeeFactory.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }
    
    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        _employeeFactory.DeleteEmployee(id);
        return RedirectToAction("Index");
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

