using Nest;

namespace EmployeeWebApp.Models;

public class EmployeeRepository: IEmployeeFactory
{
    private readonly EmployeeDbContext _dbContext;
    private readonly IElasticClient _client;
    private IEnumerable<Employee> employees;

    public EmployeeRepository(EmployeeDbContext dbContext, IElasticClient client)
    {
        _dbContext = dbContext;
        _client = client;
        employees = _dbContext.Employees.ToList();
    }

    public Employee GetEmployeeById(int id)
    {
        return _dbContext.Employees.Find(id);
    }

    public void AddEmployee(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        _dbContext.SaveChanges();
    }

    public void UpdateEmployee(Employee updatedEmployee)
    {
        var employee = _dbContext.Employees.Find(updatedEmployee.Id);
       
        employee.Name = updatedEmployee.Name;
        employee.Department = updatedEmployee.Department;
        employee.Email = updatedEmployee.Email;
        
        _dbContext.SaveChanges();
    }

    public void DeleteEmployee(int id)
    {
        var employee = _dbContext.Employees.Find(id);
        if (employee != null)
        {
            _dbContext.Employees.Remove(employee);
            _dbContext.SaveChanges();
        }
    }

    public IEnumerable<Employee> GetEmployeeList()
    {
        return employees;
    }
    
    public async Task SeedInitialData()
    {
        
        foreach (var employee in employees)
        {
            var indexResponse = await _client.IndexDocumentAsync(employee);
            if (!indexResponse.IsValid)
            {
                // Handle error if indexing fails
                Console.WriteLine($"Error indexing document: {indexResponse.DebugInformation}");
            }
        }
    }
}