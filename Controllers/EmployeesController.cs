using Employee_Manager_System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Employee_Manager_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee_Manager_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class EmployeesController : ControllerBase
    {
        private readonly AppDBContext _dbcontext;

        public EmployeesController(AppDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            if (_dbcontext.Employees == null)
            {
                return NotFound();
            }

            var employees = await _dbcontext.Employees
                .Where(e => !e.IsDeleted)
                .ToListAsync();

            return Ok(employees);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee(int Id)
        {
            if (_dbcontext.Employees == null)
            {
                return NotFound();  
            }

            var employees = await _dbcontext.Employees
                .Where(e => !e.IsDeleted && e.Id == Id)
                .FirstOrDefaultAsync();

            if (employees == null)
            {
                return NotFound();
            }

            return Ok(employees);
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployee(string name)
        {
            var employees = await _dbcontext.Employees

                .Where(e => !e.IsDeleted &&

                            (EF.Functions.Like(e.FirstName, $"%{name}%")||

                             EF.Functions.Like(e.LastName, $"%{name}%")))

                .ToListAsync();

            if (employees.Count == 0) 
            {
                return NotFound();
            }
            else
            {
                return Ok(employees);
            }
        }


        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            employee.AddedAt = DateTime.UtcNow;

            _dbcontext.Employees.Add(employee);

            await _dbcontext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }



        [HttpPut("{id}")]
        public async Task<ActionResult> PutEmployee(int id, Employee employee)
        {
            if(id != employee.Id)
            {
                return BadRequest();
            }
            _dbcontext.Entry(employee).State = EntityState.Modified;

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        private bool EmployeeAvailable(int id)
        { 
            return (_dbcontext.Employees?.Any(x => x.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var employee = await _dbcontext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.IsDeleted = true;

            _dbcontext.Entry(employee).State = EntityState.Modified;

            await _dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}
