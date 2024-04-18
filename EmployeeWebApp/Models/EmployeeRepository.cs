namespace EmployeeWebApp.Models;

public class EmployeeRepository: IEmployeeFactory
{
    private List<Employee> _employeeList;
    private readonly EmployeeDbContext _dbContext;

    public EmployeeRepository(EmployeeDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public void UpdateEmployee(int id, Employee updatedEmployee)
    {
        var employee = _dbContext.Employees.Find(id);
       
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
        return _dbContext.Employees.ToList();
    }
}