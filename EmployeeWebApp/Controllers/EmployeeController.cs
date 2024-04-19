using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeWebApp.Models;

namespace EmployeeWebApp.Controllers;

public class EmployeeController : Controller
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly IEmployeeFactory _employeeFactory;

    public EmployeeController(ILogger<EmployeeController> logger, IEmployeeFactory employeeFactory)
    {
        _logger = logger;
        _employeeFactory = employeeFactory;
    }

    public IActionResult Index()
    {
        return View(_employeeFactory.GetEmployeeList());
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

