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
public class LoadController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoadController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Load
    public IActionResult Index()
    {
        return View();
    }

    // POST: Load
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormFile zipfile)
    {
        Dictionary<string, byte[]> files = new Dictionary<string, byte[]>(); 
        Stream fs = zipfile.OpenReadStream();
        using (var ms = new MemoryStream()) 
        using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Read)) {
            files = archive.Entries.ToDictionary(
                x => x.FullName, 
                x => {
                    using(var ms = new MemoryStream()){
                        x.Open().CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            );
        }   

        foreach (var file in files) {
            var parts = file.Key.Split("-");
            if (parts.Length != 3)
                return BadRequest($"O nome do arquivo {file.Key} está fora do padrão (Departamento-Mês-Ano.csv).");
            var departmentName = parts[0];
            var mes = parts[1];
            var ano = parts[2].Replace(".csv", "");
        }    
        
        return View();
    }

    private void ProcessFile(KeyValuePair<string, byte[]> file) {
        
    }
}