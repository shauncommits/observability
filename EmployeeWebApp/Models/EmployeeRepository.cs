namespace EmployeeWebApp.Models;

public class EmployeeRepository: IEmployeeFactory
{
    private List<Employee> _employeeList;

    public EmployeeRepository()
    {
        
    }

    public Employee GetEmployeeById(int id)
    {
        throw new NotImplementedException();
    }

    public void AddEmployee(Employee employee)
    {
        throw new NotImplementedException();
    }

    public void UpdateEmployee(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteEmployee(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Employee> GetEmployeeList()
    {
        throw new NotImplementedException();
    }
}