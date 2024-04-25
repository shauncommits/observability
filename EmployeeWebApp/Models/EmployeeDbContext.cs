using Microsoft.EntityFrameworkCore;

namespace EmployeeWebApp.Models;

public class EmployeeDbContext: DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasKey(e => e.Id_Number);
    }
    public DbSet<Employee> Employees { get; set; }
}