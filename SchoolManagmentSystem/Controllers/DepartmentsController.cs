using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Data;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Departments
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
                ViewData["CreatedDateParm"] = sortOrder == "CreatedDate" ? "createddate_desc" : "CreatedDate";

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                var departments = from d in _context.Departments
                                  select d;

                if (!String.IsNullOrEmpty(searchString))
                {
                    departments = departments.Where(d => d.Name.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "Name":
                        departments = departments.OrderBy(d => d.Name);
                        break;
                    case "name_desc":
                        departments = departments.OrderByDescending(d => d.Name);
                        break;
                    case "CreatedDate":
                        departments = departments.OrderBy(d => d.CreatedDate);
                        break;
                    case "createddate_desc":
                        departments = departments.OrderByDescending(d => d.CreatedDate);
                        break;
                    default:
                        departments = departments.OrderBy(d => d.Name);
                        break;
                }

                int pageSize = 5;
                return View(await PaginatedList<Department>.CreateAsync(departments.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
        }
        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.ID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "ID", "FullName");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Department department)
        {
            department.CreatedDate = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,CreatedDate")] Department department)
        {
            if (id != department.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.ID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
          return _context.Departments.Any(e => e.ID == id);
        }

        [AllowAnonymous]
        public async Task<IActionResult> DepProfessorsList(int? id)
        {
            var professors = await _context.Professors
                .Where(p => p.DepartmentID == id)
                .ToListAsync();

            return View(professors);
        }

        [AllowAnonymous]
        public async Task<IActionResult> DepCoursesList(int? id)
        {
            var courses = await _context.Courses
                .Where(c => c.DepartmentID == id)
                .ToListAsync();

            return View(courses);
        }

    }
}

