using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employees.Data;
using Employees.Models;

namespace Employees.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchDepartment, int? ageFilter)
        {
            var employees = _context.Employees
                            .Include(e => e.Department)
                            .Where(e => e.IsActive);

            if (!string.IsNullOrEmpty(searchDepartment))
                employees = employees.Where(e => e.Department.Name.Contains(searchDepartment));

            if (ageFilter.HasValue)
                employees = employees.Where(e => e.Age > ageFilter.Value);

            // For department dropdown
            ViewData["Departments"] = new SelectList(await _context.Departments.ToListAsync(), "ID", "Name");

            
            // For stats / dashboard (employees per department)
            var stats = await _context.Employees
                            .Where(e => e.IsActive)
                            .Include(e => e.Department)
                            .GroupBy(e => e.Department.Name)
                            .Select(g => new DepartmentStat 
                            {
                                Department = g.Key,
                                Count = g.Count()
                            })
                            .ToListAsync();

            ViewData["DepartmentsCount"] = stats;
            

            return View(await employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.ID == id && m.IsActive);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.IsActive = true;
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", employee.DepartmentID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || !employee.IsActive) return NotFound();

            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", employee.DepartmentID);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.ID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", employee.DepartmentID);
            return View(employee);
        }

        // Soft Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || !employee.IsActive) return NotFound();

            employee.IsActive = false;
            _context.Update(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
