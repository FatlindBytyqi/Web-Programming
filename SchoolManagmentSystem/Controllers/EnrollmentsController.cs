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
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Enrollments
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["StudentSortParm"] = sortOrder == "Student" ? "student_desc" : "Student";
                ViewData["CourseSortParm"] = sortOrder == "Course" ? "course_desc" : "Course";

                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;


                var enrollments = from e in _context.Enrollments
                              select e;

                if (!String.IsNullOrEmpty(searchString))
                {
                    enrollments = enrollments.Where(e => e.Student.Name.Contains(searchString) || e.Course.Title.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "Student":
                        enrollments = enrollments.OrderBy(e => e.Student.Name);
                        break;
                    case "student_desc":
                        enrollments = enrollments.OrderByDescending(e => e.Student.Name);
                        break;
                    case "Course":
                        enrollments = enrollments.OrderBy(e => e.Course.Title);
                        break;
                    case "course_desc":
                        enrollments = enrollments.OrderByDescending(e => e.Course.Title);
                        break;
                    default:
                        enrollments = enrollments.OrderBy(e => e.Course.Title);
                        break;
                }

                int pageSize = 5;
                return View(await PaginatedList<Enrollment>.CreateAsync(enrollments.Include(e => e.Course).Include(e => e.Student).AsNoTracking(), pageNumber ?? 1, pageSize));
            }
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentID,CourseID,StudentID")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
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
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enrollments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Enrollments'  is null.");
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
          return (_context.Enrollments?.Any(e => e.EnrollmentID == id)).GetValueOrDefault();
        }
    }
}
