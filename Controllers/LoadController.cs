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
    public async Task<IActionResult> Index(IFormFile File)
    {
        Dictionary<string, byte[]> files = new Dictionary<string, byte[]>(); 
        
        using (var ms = new MemoryStream()) {
            await File.CopyToAsync(ms);
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
        }

        foreach (var file in files) {
            var status = await ProcessFile(file);
        }    
        
        return View();
    }

    private async Task<string> ProcessFile(KeyValuePair<string, byte[]> file) {
        var parts = file.Key.Split("-");
        if (parts.Length != 3)
            return $"O nome do arquivo {file.Key} está fora do padrão (Departamento-Mês-Ano.csv).";
        var departmentName = parts[0];
        var mes = parts[1];
        var ano = parts[2].Replace(".csv", "");

        var department = _context.Departments.FirstOrDefault(x => x.Name == departmentName);
        if (department == null) {
            department = _context.Departments.Add(new Department { Name = departmentName } ).Entity;
            await _context.SaveChangesAsync();
        }
        
        string st = System.Text.Encoding.UTF8.GetString(file.Value, 0, file.Value.Length);
        var lines = st.Split("\n");
        Dictionary<long, Employee> employees = new Dictionary<long, Employee>();
        var clockIns = new List<ClockInsInputDTO>();
        for (var i = 1; i < lines.Length; i++) {
            if (lines[i] != null) {
                var l = lines[i].Split(";");
                var d = l[3].Split("/");
                DateTime date = new DateTime(int.Parse(d[2]), int.Parse(d[1]), int.Parse(d[0]));
                var e = l[4].Split(":");
                var entrada = new DateTime(date.Year, date.Month, date.Day, int.Parse(e[0]), int.Parse(e[1]), int.Parse(e[2]));
                var s = l[5].Split(":");
                var saida = new DateTime(date.Year, date.Month, date.Day, int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
                var alm = l[6].Split(" - ");
                var iAlm = alm[0].Split(":");
                var vAlm = alm[1].Split(":");
                Console.WriteLine(l[2].Replace("R$", "").Replace(" ", "").Replace(",", "."));

                var c = new ClockInsInputDTO {
                    Codigo = long.Parse(l[0]),
                    Nome = l[1],
                    ValorHora = double.Parse(l[2].Replace("R$", "").Replace(" ", "").Replace(",", "."), CultureInfo.InvariantCulture),
                    Data = date,
                    Entrada = entrada,
                    Saida = saida,
                    IdaAlmoco = new DateTime(date.Year, date.Month, date.Day, int.Parse(iAlm[0]), int.Parse(iAlm[1]), 00),
                    VoltaAlmoco = new DateTime(date.Year, date.Month, date.Day, int.Parse(vAlm[0]), int.Parse(vAlm[1]), 00)
                };
                clockIns.Add(c);
            }
        }
        foreach (var clockIn in clockIns) {
            employees.TryAdd(clockIn.Codigo, new Employee {
                Id = clockIn.Codigo,
                Name = clockIn.Nome,
                HourlyRate = clockIn.ValorHora,
                DepartmentId = department.Id,
                ClockIns = new List<ClockIn>()
            });
            if (employees[clockIn.Codigo].ClockIns == null) 
                employees[clockIn.Codigo].ClockIns = new List<ClockIn>();
            
            employees[clockIn.Codigo].ClockIns.Add(new ClockIn {
                Entry = clockIn.Entrada,
                Exit = clockIn.Saida,
                EntryLunch = clockIn.IdaAlmoco,
                ExitLunch = clockIn.VoltaAlmoco,
                EmployeeId = clockIn.Codigo
            });
            
        }

        foreach (var employee in employees) {
            var empl = _context.Employees.FirstOrDefault(x => x.Name == employee.Value.Name);
            if (empl == null) {
                empl = _context.Employees.Add(new Employee {
                    Name = employee.Value.Name,
                    HourlyRate = employee.Value.HourlyRate,
                    DepartmentId = employee.Value.DepartmentId
                }).Entity;
                await _context.SaveChangesAsync();
            }
            employee.Value.Id = empl.Id;
            foreach (var clockin in employee.Value.ClockIns) {
                clockin.EmployeeId = employee.Value.Id;
                _context.Add(clockin);
                await _context.SaveChangesAsync();
            }
        }
        await _context.SaveChangesAsync();   
        return $"Arquivo {file.Key} processado com sucesso.";
    }
}