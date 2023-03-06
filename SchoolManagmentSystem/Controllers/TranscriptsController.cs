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
    public class Transcripts : Controller
    {
        private readonly ApplicationDbContext _context;

        public Transcripts(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Transcripts
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            {
                ViewData["CurrentSort"] = sortOrder;
                ViewData["GradeSortParm"] = sortOrder == "Grade" ? "grade_desc" : "Grade";
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


                var transcripts = from t in _context.Transcript
                              select t;

                if (!String.IsNullOrEmpty(searchString))
                {
                    transcripts = transcripts.Where(t => t.Student.Name.Contains(searchString) || t.Course.Title.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "Grade":
                        transcripts = transcripts.OrderBy(t => t.Grade);
                        break;
                    case "grade_desc":
                        transcripts = transcripts.OrderByDescending(t => t.Grade);
                        break;
                    case "Student":
                        transcripts = transcripts.OrderBy(t => t.Student.Name);
                        break;
                    case "student_desc":
                        transcripts = transcripts.OrderByDescending(t => t.Student.Name);
                        break;
                    case "Course":
                        transcripts = transcripts.OrderBy(t => t.Course.Title);
                        break;
                    case "course_desc":
                        transcripts = transcripts.OrderByDescending(t => t.Course.Title);
                        break;
                    default:
                        transcripts = transcripts.OrderBy(t => t.Course.Title);
                        break;
                }

                int pageSize = 5;
                return View(await PaginatedList<Transcript>.CreateAsync(transcripts.Include(t => t.Course).Include(t => t.Student).AsNoTracking(), pageNumber ?? 1, pageSize));
            }    
        }

        // GET: Transcripts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transcript == null)
            {
                return NotFound();
            }

            var transcript = await _context.Transcript
                .Include(t => t.Course)
                .Include(t => t.Student)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (transcript == null)
            {
                return NotFound();
            }

            return View(transcript);
        }

        // GET: Transcripts/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName");
            return View();
        }

        // POST: Transcripts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Grade,StudentID,CourseID")] Transcript transcript)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transcript);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", transcript.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName", transcript.StudentID);
            return View(transcript);
        }

        // GET: Transcripts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transcript == null)
            {
                return NotFound();
            }

            var transcript = await _context.Transcript.FindAsync(id);
            if (transcript == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", transcript.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName", transcript.StudentID);
            return View(transcript);
        }

        // POST: Transcripts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Grade,StudentID,CourseID")] Transcript transcript)
        {
            if (id != transcript.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transcript);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TranscriptExists(transcript.ID))
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
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", transcript.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "FullName", transcript.StudentID);
            return View(transcript);
        }

        // GET: Transcripts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transcript == null)
            {
                return NotFound();
            }

            var transcript = await _context.Transcript
                .Include(t => t.Course)
                .Include(t => t.Student)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (transcript == null)
            {
                return NotFound();
            }

            return View(transcript);
        }

        // POST: Transcripts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transcript == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transcript'  is null.");
            }
            var transcript = await _context.Transcript.FindAsync(id);
            if (transcript != null)
            {
                _context.Transcript.Remove(transcript);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TranscriptExists(int id)
        {
          return (_context.Transcript?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
