using Nest;
using Bogus;


namespace EmployeeWebApp.Models;

public class EmployeeRepository: IEmployeeFactory
{
    private readonly EmployeeDbContext _dbContext;
    private readonly IElasticClient _client;
    private IEnumerable<Employee> employees;
    private long count;

    public EmployeeRepository(EmployeeDbContext dbContext, IElasticClient client)
    {
        _dbContext = dbContext;
        _client = client;
        employees = _dbContext.Employees.ToList();
        // InitializeAsync().Wait();
    }
    
    private async Task InitializeAsync()
    {
        await SeedInitialData();
    }

    public Employee GetEmployeeById(long id)
    {
        // Get employee from the database
        return employees.FirstOrDefault(e => e.Id_Number == id);
    }

    public void AddEmployee(Employee employee)
    {
        count = GetMaxId() + 1;
        employee.Id_Number = count;
        _dbContext.Employees.Add(employee);
        _dbContext.SaveChanges();
    }

    public void UpdateEmployee(Employee updatedEmployee)
    {
        // Updating the employee on the database
        var employee = employees.FirstOrDefault(e => e.Id_Number == updatedEmployee.Id_Number);
        
        employee.Name = updatedEmployee.Name;
        employee.Department = updatedEmployee.Department;
        employee.Email = updatedEmployee.Email;
        
        _dbContext.SaveChanges();
    }
    
    public long GetMaxId()
    {
        var maxId = _dbContext.Employees.Max(e => e.Id_Number);
        return maxId;
    }

    public void DeleteEmployee(long id)
    {
        
        // Delete Employee from the database
        var employee = employees.FirstOrDefault(e => e.Id_Number == id);
        if (employee != null)
        {
            _dbContext.Employees.Remove(employee);
            _dbContext.SaveChanges();
        }
    }

    public IEnumerable<Employee> GetEmployeeList()
    {
        // Get list of employees from the database
        return employees;
    }
    
    public async Task SeedInitialData()
    {
        // Create a Faker instance
        var faker = new Faker();

        // Generate a specific number of random employee data
        int numberOfEmployees = 1000;

        for (long i = 1; i <= numberOfEmployees; i++)
        {
            var employee = new Employee
            {
                Id_Number = i,
                Name = faker.Name.FirstName(),
                Department = (Department)faker.Random.Number(0, 7),
                Email = faker.Internet.Email()
            };

            AddEmployee(employee);

            var indexResponse = await _client.IndexDocumentAsync(employee);
            if (!indexResponse.IsValid)
            {
                // Handle error if indexing fails
                Console.WriteLine($"Error indexing document: {indexResponse.DebugInformation}");
            }
        }
    }
}