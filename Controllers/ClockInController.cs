using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using testeAuvo.Data;
using testeAuvo.Models;

namespace testeAuvo.Controllers;
[Authorize]
public class ClockInController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClockInController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ClockIn
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.ClockIns.Include(c => c.Employee);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: ClockIn/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null || _context.ClockIns == null)
        {
            return NotFound();
        }

        var clockIn = await _context.ClockIns
            .Include(c => c.Employee)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (clockIn == null)
        {
            return NotFound();
        }

        return View(clockIn);
    }

    // GET: ClockIn/Create
    public IActionResult Create()
    {
        ViewData["Employee"] = new SelectList(_context.Employees, "Name", "Name");
        return View();
    }

    // POST: ClockIn/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Entry,Exit,EntryLunch,ExitLunch")] ClockIn clockIn, string employee)
    {
        Employee empl = _context.Employees.AsQueryable().First(x => x.Name == employee);
        clockIn.EmployeeId = empl.Id;
        Console.WriteLine(clockIn.EmployeeId);
        if (ModelState.IsValid)
        {
            _context.Add(clockIn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", clockIn.EmployeeId);
        return View(clockIn);
    }

    // GET: ClockIn/Load
    public IActionResult Load()
    {
        return View();
    }

    // POST: ClockIn/Load
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Load([Bind("file")] IFormFile file)
    {
        Console.WriteLine(file.Length);
        return View();
    }

    // GET: ClockIn/Edit/5
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null || _context.ClockIns == null)
        {
            return NotFound();
        }

        var clockIn = await _context.ClockIns.FindAsync(id);
        if (clockIn == null)
        {
            return NotFound();
        }
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", clockIn.EmployeeId);
        return View(clockIn);
    }

    // POST: ClockIn/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, [Bind("Id,Entry,Exit,EntryLunch,ExitLunch,EmployeeId")] ClockIn clockIn)
    {
        if (id != clockIn.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(clockIn);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClockInExists(clockIn.Id))
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
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", clockIn.EmployeeId);
        return View(clockIn);
    }

    // GET: ClockIn/Delete/5
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null || _context.ClockIns == null)
        {
            return NotFound();
        }

        var clockIn = await _context.ClockIns
            .Include(c => c.Employee)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (clockIn == null)
        {
            return NotFound();
        }

        return View(clockIn);
    }

    // POST: ClockIn/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        if (_context.ClockIns == null)
        {
            return Problem("Entity set 'ApplicationDbContext.ClockIns'  is null.");
        }
        var clockIn = await _context.ClockIns.FindAsync(id);
        if (clockIn != null)
        {
            _context.ClockIns.Remove(clockIn);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ClockInExists(long id)
    {
        return (_context.ClockIns?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}