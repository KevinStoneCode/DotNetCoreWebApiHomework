using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetCoreWebApiHomework.Models;

namespace DotNetCoreWebApiHomework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            try
            {
                var departs = _context.Department.FromSqlInterpolated($"EXECUTE dbo.Department_Update {department.DepartmentId},{department.Name},{department.Budget},{department.StartDate},{department.InstructorId},{department.RowVersion}")
                .IgnoreQueryFilters()
                .Select(x => new Department() { RowVersion = x.RowVersion })
                .ToList();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            // _context.Department.Add(department);
            // await _context.SaveChangesAsync();

            var depart = _context.Department.FromSqlInterpolated($"EXECUTE dbo.Department_Insert {department.Name},{department.Budget},{department.StartDate},{department.InstructorId}")
                .IgnoreQueryFilters()
                .Select(x => new { x.DepartmentId, x.RowVersion })
                .AsEnumerable()
                .FirstOrDefault();
            department.DepartmentId = depart.DepartmentId;
            department.RowVersion = depart.RowVersion;

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            //_context.Department.Remove(department);
            //await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlInterpolatedAsync($"EXECUTE dbo.Department_Delete {department.DepartmentId},{department.RowVersion}");
            _context.Entry(department).Reload();

            return department;
        }

        // GET: api/Departments/DepartmentCourseCount
        [HttpGet("DepartmentCourseCount")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetDepartmentCourseCount()
        {
            var departmentCourseCount = await _context.VwDepartmentCourseCount.FromSqlRaw($"SELECT * FROM dbo.vwDepartmentCourseCount")
                .ToListAsync();

            return departmentCourseCount;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
