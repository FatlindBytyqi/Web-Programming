using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Data;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
                ViewData["SurnameSortParm"] = sortOrder == "Surname" ? "surname_desc" : "Surname";
                ViewData["RegisterDateSortParm"] = sortOrder == "RegisterDate" ? "registerdate_desc" : "RegisterDate";
                ViewData["BirthDateSortParm"] = sortOrder == "BirthDate" ? "birthdate_desc" : "BirthDate";

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;

                var students = from s in _context.Students
                               select s;

                if (!String.IsNullOrEmpty(searchString))
                {
                    students = students.Where(s => s.Name.Contains(searchString)
                                           || s.Surname.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "Name":
                        students = students.OrderBy(s => s.Name);
                        break;
                    case "name_desc":
                        students = students.OrderByDescending(s => s.Name);
                        break;
                    case "Surname":
                        students = students.OrderBy(s => s.Surname);
                        break;
                    case "surname_desc":
                        students = students.OrderByDescending(s => s.Surname);
                        break;
                    case "RegisterDate":
                        students = students.OrderBy(s => s.RegisterDate);
                        break;
                    case "registerdate_desc":
                        students = students.OrderByDescending(s => s.RegisterDate);
                        break;
                    case "BirthDate":
                        students = students.OrderBy(s => s.BirthDate);
                        break;
                    case "birthdate_desc":
                        students = students.OrderByDescending(s => s.BirthDate);
                        break;
                    default:
                        students = students.OrderBy(s => s.Name);
                        break;
                }

                int pageSize = 5;
                return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Surname,Email,BirthDate,RegisterDate,Address")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Surname,Email,BirthDate,RegisterDate,Address")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.ID == id);
        }
    }
}
