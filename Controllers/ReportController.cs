using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using testeAuvo.Data;
using testeAuvo.DTOs;
using testeAuvo.Models;

namespace testeAuvo.Controllers;
[Authorize]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Report
    public IActionResult Index()
    {
        return View();
    }

    // POST: Report
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([Bind("Month,Year")] Report report)
    {
        var departments = _context.Departments.Include(x => x.Employees).ThenInclude(x => x.ClockIns);
        return View(departments);
    }
}