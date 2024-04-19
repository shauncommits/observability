﻿using System.Diagnostics;
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
        _employeeFactory.SeedInitialData();
    }

    public IActionResult Index(string searchQuery)
    {
        IEnumerable<Employee> employees;

        if (!string.IsNullOrEmpty(searchQuery))
        {

            // Perform the search using the NEST package
            var searchResponse = _client.Search<Employee>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(searchQuery)
                    )
                )
            );

            // Get the search results
            employees = searchResponse.Documents;
        }
        else
        {
            // Retrieve all employees if no search query is provided
            employees = _employeeFactory.GetEmployeeList();
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

