namespace EmployeeWebApp.Models;

public interface IEmployeeFactory
{
    Employee GetEmployeeById(int id);
    void AddEmployee(Employee employee);
    void UpdateEmployee(int id, Employee employee);
    void DeleteEmployee(int id);
    IEnumerable<Employee> GetEmployeeList();
}