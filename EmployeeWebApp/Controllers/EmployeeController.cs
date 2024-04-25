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
    private readonly ActivitySource _activitySource = new ActivitySource("EmployeeWebAppActivitySource");

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
                    using (var activity = _activitySource.StartActivity("SearchInfor"))
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

                        var wildcardQuery = $"*{searchQuery}*";
                        activity?.SetTag("search.value", searchQuery);

                        // Propagate the tag to the spans
                        using (var searchSpan = _activitySource.StartActivity("Search", ActivityKind.Client))
                        {
                            searchSpan?.SetTag("search.value", searchQuery);
                            
                            var searchResponse = _client.Search<Employee>(s => s
                                    .Query(q => q
                                        .QueryString(qs => qs
                                            .Query(wildcardQuery)
                                        )
                                    )
                                    .Size(100) // Retrieve top 100 match search queries
                            );

                            employees = searchResponse.Documents;
                        }
                    }
                }
                else
                {
                    // Retrieve all employees if no search query is provided

                    ///// Uncomment this to show how is slow to retrieve data from a database compared using elastic search and also remove async Task<IActionResult> to just IActionResult as return type 
                    // employees = _employeeFactory.GetEmployeeList();

                    ISearchResponse<Employee> searchResponse = await _client.SearchAsync<Employee>(s => s
                        .MatchAll()
                        .Index("employee_web_app").Size(10000));

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
            // Adding the employee to the database
            _employeeFactory.AddEmployee(employee);
            
            // Adding the employee to an elastic search datasource
            var indexResponse = _client.IndexDocument(employee);
            if (!indexResponse.IsValid)
            {
                // Handle error
                throw new Exception($"Error adding employee: {indexResponse.DebugInformation}");
            }
            
            return RedirectToAction("Index");
        }
        return View(employee);
    }
    
    public IActionResult Edit(long id)
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
            // Updating an employee from the database
            _employeeFactory.UpdateEmployee(employee);
            
            var fieldValue = employee.Id_Number;

            var searchResponse = _client.Search<Employee>(s => s
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.Id_Number)
                        .Value(fieldValue)
                    )
                )
                .Source(src => src
                    .Includes(i => i
                        .Field("_id")
                    )
                )
                .Size(1)
            );
            
            var documentId = searchResponse.Hits.FirstOrDefault()?.Id;
            
            // Updating an employee from Elasticsearch
            var updateResponse = _client.Update<Employee, object>(documentId, u => u
                .Doc(employee)
                .Index("employee_web_app")
            );

            
            if (!updateResponse.IsValid)
            {
                // Handle error
                throw new Exception($"Error updating employee: {updateResponse.DebugInformation}");
            }

            
            
            return RedirectToAction("Index");
        }
        return View(employee);
    }
    
    public IActionResult Delete(long id)
    {
        var employee = _employeeFactory.GetEmployeeById(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }
    
    [HttpPost]
    public IActionResult DeleteConfirmed(long id)
    {
        // Deleting employee from the elastic search datasource
        
        var searchResponse = _client.Search<Employee>(s => s
            .Query(q => q
                .Term(t => t
                    .Field(f => f.Id_Number)
                    .Value(id)
                )
            )
            .Source(src => src
                .Includes(i => i
                    .Field("_id")
                )
            )
            .Size(1)
        );
            
        var documentId = searchResponse.Hits.FirstOrDefault()?.Id;
        var deleteResponse = _client.Update<Employee, object>(documentId, u => u
            .Doc(new { deleted = true })
        );
        
        // Deleting employee from the database
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

