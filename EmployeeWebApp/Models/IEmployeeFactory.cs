namespace EmployeeWebApp.Models;

public interface IEmployeeFactory
{
    Employee GetEmployeeById(long id);
    void AddEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void DeleteEmployee(long id);
    IEnumerable<Employee> GetEmployeeList();
    Task SeedInitialData();
}