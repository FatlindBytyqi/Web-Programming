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
    public class AssistantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssistantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Assistants
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
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


            var assistants = from a in _context.Assistants
                             select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                assistants = assistants.Where(a => a.Name.Contains(searchString)
                                       || a.Surname.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Name":
                    assistants = assistants.OrderBy(a => a.Name);
                    break;
                case "name_desc":
                    assistants = assistants.OrderByDescending(a => a.Name);
                    break;
                case "Surname":
                    assistants = assistants.OrderBy(a => a.Surname);
                    break;
                case "surname_desc":
                    assistants = assistants.OrderByDescending(a => a.Surname);
                    break;
                case "HireDate":
                    assistants = assistants.OrderBy(a => a.HireDate);
                    break;
                case "hiredate_desc":
                    assistants = assistants.OrderByDescending(a => a.HireDate);
                    break;
                case "BirthDate":
                    assistants = assistants.OrderBy(a => a.BirthDate);
                    break;
                case "birthdate_desc":
                    assistants = assistants.OrderByDescending(a => a.BirthDate);
                    break;
                default:
                    assistants = assistants.OrderBy(a => a.Name);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Assistant>.CreateAsync(assistants.Include(a => a.Professor).AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Assistants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Assistants == null)
            {
                return NotFound();
            }

            var assistant = await _context.Assistants
                .Include(a => a.Professor)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (assistant == null)
            {
                return NotFound();
            }

            return View(assistant);
        }

        // GET: Assistants/Create
        public IActionResult Create()
        {
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "ID", "FullName");
            return View();
        }

        // POST: Assistants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Surname,Email,BirthDate,HireDate,Address, ProfessorID")] Assistant assistant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assistant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "ID", "FullName", assistant.ProfessorID);
            return View(assistant);
        }

        // GET: Assistants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Assistants == null)
            {
                return NotFound();
            }

            var assistant = await _context.Assistants.FindAsync(id);
            if (assistant == null)
            {
                return NotFound();
            }
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "ID", "FullName", assistant.ProfessorID);
            return View(assistant);
        }

        // POST: Assistants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Surname,Email,BirthDate,HireDate,Address, ProfessorID")] Assistant assistant)
        {
            if (id != assistant.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assistant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssistantExists(assistant.ID))
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
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "ID", "FullName", assistant.ProfessorID);
            return View(assistant);
        }

        // GET: Assistants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Assistants == null)
            {
                return NotFound();
            }

            var assistant = await _context.Assistants
                .FirstOrDefaultAsync(m => m.ID == id);
            if (assistant == null)
            {
                return NotFound();
            }

            return View(assistant);
        }

        // POST: Assistants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Assistants == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Assistants'  is null.");
            }
            var assistant = await _context.Assistants.FindAsync(id);
            if (assistant != null)
            {
                _context.Assistants.Remove(assistant);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssistantExists(int id)
        {
          return (_context.Assistants?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
