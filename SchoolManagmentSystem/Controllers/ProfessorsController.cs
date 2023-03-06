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
    public class ProfessorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfessorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Professors
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
                ViewData["SurnameSortParm"] = sortOrder == "Surname" ? "surname_desc" : "Surname";
                ViewData["HireDateSortParm"] = sortOrder == "HireDate" ? "hiredate_desc" : "HireDate";
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


                var professors = from p in _context.Professors
                                 select p;

                if (!String.IsNullOrEmpty(searchString))
                {
                    professors = professors.Where(p => p.Name.Contains(searchString)
                                           || p.Surname.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "Name":
                        professors = professors.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        professors = professors.OrderByDescending(p => p.Name);
                        break;
                    case "Surname":
                        professors = professors.OrderBy(p => p.Surname);
                        break;
                    case "surname_desc":
                        professors = professors.OrderByDescending(p => p.Surname);
                        break;
                    case "HireDate":
                        professors = professors.OrderBy(p => p.HireDate);
                        break;
                    case "hiredate_desc":
                        professors = professors.OrderByDescending(p => p.HireDate);
                        break;
                    case "BirthDate":
                        professors = professors.OrderBy(p => p.BirthDate);
                        break;
                    case "birthdate_desc":
                        professors = professors.OrderByDescending(p => p.BirthDate);
                        break;
                    default:
                        professors = professors.OrderBy(p => p.Name);
                        break;
                }

                int pageSize = 5;
                return View(await PaginatedList<Professor>.CreateAsync(professors.Include(p => p.Department).AsNoTracking(), pageNumber ?? 1, pageSize));
            }
        }

        // GET: Professors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // GET: Professors/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name");
            return View();
        }

        // POST: Professors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Surname,Email,BirthDate,HireDate,Address,DepartmentID")] Professor professor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(professor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", professor.DepartmentID);
            return View(professor);
        }

        // GET: Professors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", professor.DepartmentID);
            return View(professor);
        }

        // POST: Professors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Surname,Email,BirthDate,HireDate,Address,DepartmentID")] Professor professor)
        {
            if (id != professor.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(professor.ID))
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
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", professor.DepartmentID);
            return View(professor);
        }

        // GET: Professors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // POST: Professors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Professors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Professors'  is null.");
            }
            var professor = await _context.Professors.FindAsync(id);
            if (professor != null)
            {
                _context.Professors.Remove(professor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
          return _context.Professors.Any(e => e.ID == id);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProfAssistList(int? id)
        {
            var assistants = await _context.Assistants
                .Where(a => a.ProfessorID == id)
                .ToListAsync();

            return View(assistants);
        }

    }
}
