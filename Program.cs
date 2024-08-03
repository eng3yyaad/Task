using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace ConsoleApp3
{
    //public class AdventureWorksContext : DbContext
    //{
    //    public DbSet<Employee> Employees { get; set; }
    //    public DbSet<Department> Departments { get; set; }
    //    public DbSet<Shift> Shifts { get; set; }
    //    public DbSet<Person> Person { get; set; }
    //    public DbSet<EmployeeDepartmentHistory> EmployeeDepartmentHistory { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        var connectionString = Environment.GetEnvironmentVariable("AdventureWorksConnectionString");
    //        if (string.IsNullOrEmpty(connectionString))
    //        {
    //            throw new InvalidOperationException("Connection string not found in environment variables.");
    //        }

    //        optionsBuilder
    //            .UseSqlServer(connectionString)
    //            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
    //            .EnableSensitiveDataLogging();
    //    }
    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<Employee>()
    //         .HasKey(e => e.BusinessEntityId);

    //        modelBuilder.Entity<Person>()
    //            .HasKey(p => p.BusinessEntityId);

    //        modelBuilder.Entity<Employee>()
    //            .HasOne(e => e)
    //            .WithOne(p => p.Employee)
    //            .HasForeignKey<Person>(p => p.BusinessEntityID);

    //        modelBuilder.Entity<EmployeeDepartmentHistory>()
    //            .HasKey(edh => new { edh.BusinessEntityID, edh.DepartmentID, edh.StartDate });

    //        modelBuilder.Entity<EmployeeDepartmentHistory>()
    //            .HasOne(edh => edh.Employee)
    //            .WithMany(e => e.EmployeeDepartmentHistories)
    //            .HasForeignKey(edh => edh.BusinessEntityID);

    //        modelBuilder.Entity<EmployeeDepartmentHistory>()
    //            .HasOne(edh => edh.Department)
    //            .WithMany(d => d.EmployeeDepartmentHistories)
    //            .HasForeignKey(edh => edh.DepartmentID);

    //        modelBuilder.Entity<Department>()
    //            .HasKey(d => d.DepartmentID);
    //    }
    //}

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Environment.SetEnvironmentVariable("AdventureWorksConnectionString", "Data Source=.;Initial Catalog=AdventureWorks2019;Integrated Security=True;TrustServerCertificate=True");


            await SelectAllEmployees();
            await SelectAllDepartments();
            await SelectAllShifts();
        }
        static async Task SelectAllEmployees()
        {
            using var context = new AdventureWorks2019Context();
            var query = from e in context.Employees
                        join p in context.Person on e.BusinessEntityId equals p.BusinessEntityId
                        join de in context.EmployeeDepartmentHistories on e.BusinessEntityId equals de.BusinessEntityId
                        join d in context.Departments on de.DepartmentId equals d.DepartmentId
                        select new
                        {
                            EmployeeId = e.BusinessEntityId,
                            Name = $"{p.FirstName} {p.MiddleName} {p.LastName}",
                            JobTitle = e.JobTitle,
                            Department = d.Name,
                            Gender = e.Gender,
                            HireDate = e.HireDate
                        };

            var result = await query.ToListAsync();

            foreach (var employee in result)
            {
                Console.WriteLine($"{employee.EmployeeId}, {employee.Name}, {employee.JobTitle}, {employee.Department}, {employee.HireDate},{employee.Gender}");
            }
        }
        static async Task SelectAllDepartments()
        {
            using var context = new AdventureWorks2019Context();
            var query = from d in context.Departments
                        join de in context.EmployeeDepartmentHistories on d.DepartmentId equals de.DepartmentId
                        group de by new { d.DepartmentId, d.Name } into departmentGroup
                        select new
                        {
                            DepartmentId = departmentGroup.Key.DepartmentId,
                            DepartmentName = departmentGroup.Key.Name,
                            TotalEmployees = departmentGroup.Count()
                        };

            var result = await query.ToListAsync();

            foreach (var item in result)
            {
                Console.WriteLine($"Department ID: {item.DepartmentId}, Department Name: {item.DepartmentName}, Total Employees: {item.TotalEmployees}");
            }
        }
        static async Task SelectAllShifts()
        {
            using var context = new AdventureWorks2019Context();
            var query = from s in context.Shifts
                        join de in context.EmployeeDepartmentHistories on s.ShiftId equals de.ShiftId
                        join pe in context.Person on de.BusinessEntityId equals pe.BusinessEntityId
                        //group de by  { s.ShiftId, s.Name } 
                        select new
                        {
                            shiftId = s.ShiftId,
                            shiftName = s.Name,
                            StartTime=s.StartTime,
                            EndTime=s.EndTime,
                            Employees=pe.FirstName
                        };

            var result = await query.ToListAsync();

            foreach (var item in result)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"shift Id: {item.shiftId}, shift Name: {item.shiftName},: Start Time {item.StartTime}, End Time: {item.EndTime},employees : {item.Employees}");
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"employees : {item.Employees}");
            }

        }
    }
}

