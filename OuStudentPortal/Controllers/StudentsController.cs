using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OuStudentPortal.Data;  // Make sure this namespace points to your ApplicationDbContext
using OuStudentPortal.Models;

namespace OuStudentPortal.Controllers;

public class StudentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Students
    public async Task<IActionResult> Index()
    {
        var students = await _context.Students.OrderBy(s => s.Id).ToListAsync();
        return View(students);
    }

    // GET: /Students/Details/2
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return NotFound();
        return View(student);
    }

    // GET: /Students/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Students/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: /Students/Edit/2
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();
        return View(student);
    }

    // POST: /Students/Edit/2
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.Id == student.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: /Students/Delete/2
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
        if (student == null) return NotFound();
        return View(student);
    }

    // POST: /Students/Delete/2
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
