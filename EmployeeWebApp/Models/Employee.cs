namespace EmployeeWebApp.Models;

public class Employee
{
    public long Id_Number { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Department? Department { get; set; }
}