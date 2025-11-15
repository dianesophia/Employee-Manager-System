using Microsoft.EntityFrameworkCore;
using Employee_Manager_System.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Employee_Manager_System.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasColumnType("decimal(18,2)");
        }
    }
}
