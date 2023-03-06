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
    public class DeptBranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeptBranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: DeptBranches
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["DeptSortParm"] = sortOrder == "Department" ? "department_desc" : "Department";
                ViewData["BranchSortParm"] = sortOrder == "Branch" ? "branches_desc" : "Branch";

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;


                var deptbranches = from d in _context.DeptBranch
                              select d;

                if (!String.IsNullOrEmpty(searchString))
                {
                    deptbranches = deptbranches.Where(d => d.Branch.Name.Contains(searchString)
                    || d.Department.Name.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "Branch":
                        deptbranches = deptbranches.OrderBy(d => d.Branch);
                        break;
                    case "branches_desc":
                        deptbranches = deptbranches.OrderByDescending(d => d.Branch);
                        break;
                    case "Department":
                        deptbranches = deptbranches.OrderBy(d => d.Department);
                        break;
                    case "department_desc":
                        deptbranches = deptbranches.OrderByDescending(d => d.Department);
                        break;
                    default:
                        deptbranches = deptbranches.OrderBy(d => d.Branch);
                        break;
                }

                int pageSize = 5;
                return View(await PaginatedList<DeptBranch>.CreateAsync(deptbranches.Include(d => d.Branch ).Include(d=>d.Department).AsNoTracking(), pageNumber ?? 1, pageSize));
            }
        }

        // GET: DeptBranches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DeptBranch == null)
            {
                return NotFound();
            }

            var deptBranch = await _context.DeptBranch
                .Include(d => d.Branch)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (deptBranch == null)
            {
                return NotFound();
            }

            return View(deptBranch);
        }

        // GET: DeptBranches/Create
        public IActionResult Create()
        {
            ViewData["BranchID"] = new SelectList(_context.Branches, "BranchID", "Name");
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name");
            return View();
        }

        // POST: DeptBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,BranchID")] DeptBranch deptBranch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deptBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchID"] = new SelectList(_context.Branches, "BranchID", "Name", deptBranch.BranchID);
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", deptBranch.DepartmentID);
            return View(deptBranch);
        }

        // GET: DeptBranches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DeptBranch == null)
            {
                return NotFound();
            }

            var deptBranch = await _context.DeptBranch.FindAsync(id);
            if (deptBranch == null)
            {
                return NotFound();
            }
            ViewData["BranchID"] = new SelectList(_context.Branches, "BranchID", "Name", deptBranch.BranchID);
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", deptBranch.DepartmentID);
            return View(deptBranch);
        }

        // POST: DeptBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,BranchID")] DeptBranch deptBranch)
        {
            if (id != deptBranch.DepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deptBranch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeptBranchExists(deptBranch.DepartmentID))
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
            ViewData["BranchID"] = new SelectList(_context.Branches, "BranchID", "BranchID", deptBranch.BranchID);
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "ID", "Name", deptBranch.DepartmentID);
            return View(deptBranch);
        }

        // GET: DeptBranches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DeptBranch == null)
            {
                return NotFound();
            }

            var deptBranch = await _context.DeptBranch
                .Include(d => d.Branch)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (deptBranch == null)
            {
                return NotFound();
            }

            return View(deptBranch);
        }

        // POST: DeptBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DeptBranch == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DeptBranch'  is null.");
            }
            var deptBranch = await _context.DeptBranch.FindAsync(id);
            if (deptBranch != null)
            {
                _context.DeptBranch.Remove(deptBranch);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeptBranchExists(int id)
        {
          return _context.DeptBranch.Any(e => e.DepartmentID == id);
        }
    }
}
