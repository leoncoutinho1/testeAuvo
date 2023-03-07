using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using testeAuvo.Data;
using testeAuvo.DTOs;
using testeAuvo.Enums;
using testeAuvo.Models;

namespace testeAuvo.Controllers;
[Authorize]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly object _lock = new object();

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Report
    public IActionResult Index()
    {
        ViewData["Months"] = new SelectList(Enum.GetValues(typeof(Months)));
        return View();
    }

    // POST: Report
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([Bind("Mes","Ano")] Report report)
    {
        var departments = new List<DepartmentDTO>();
        var depts = await _context.Departments.ToListAsync();
               
        foreach (var dept in depts) {
            departments.Add(new DepartmentDTO {
                Departamento = dept.Name,
                Codigo = dept.Id,
                MesVigencia = report.Mes,
                AnoVigencia = report.Ano,
                TotalPagar = 0,
                TotalDescontos = 0,
                TotalExtras = 0,
                Funcionarios = new List<EmployeeDTO>()
            });
        }

        foreach (var department in departments) {
            var empls = await _context.Employees.Where(x => x.DepartmentId == department.Codigo).ToListAsync();
            foreach (var empl in empls) {
                department.Funcionarios.Add(new EmployeeDTO {
                    Nome = empl.Name,
                    Codigo = empl.Id,
                    HourlyRate = empl.HourlyRate,
                    TotalReceber = 0,
                    HorasExtras = 0,
                    HorasDebito = 0,
                    DiasFalta = 0,
                    DiasExtras = 0,
                    DiasTrabalhados = 0
                });
            }

            foreach (var employee in department.Funcionarios) {
                var HourlyRate = employee.HourlyRate;
                var clocks = await _context.ClockIns.Where(x => x.Entry.Month == ((int)report.Mes) && x.Entry.Year == report.Ano && x.EmployeeId == employee.Codigo).ToListAsync();
                var dataInicio = new DateTime(report.Ano, ((int)report.Mes), 01, 00, 00, 00);
                var dataFim = new DateTime(report.Ano, ((int)report.Mes), DateTime.DaysInMonth(report.Ano, ((int)report.Mes)), 23, 59, 59);
                
                
                var dataAtual = dataInicio;
                while (dataAtual <= dataFim) {
                    var clockin = clocks.FirstOrDefault(x => x.Entry.Date == dataAtual);
                    
                    bool diaTrabalhado = false;
                    int horasTrabalhadas = 0;
                    if (clockin != null) {
                        diaTrabalhado = true;                
                        horasTrabalhadas = (clockin.Exit - clockin.Entry - (clockin.ExitLunch - clockin.EntryLunch)).Hours;
                    }

                    if (dataAtual.DayOfWeek != DayOfWeek.Saturday && dataAtual.DayOfWeek != DayOfWeek.Sunday) {
                        employee.DiasTrabalhados += (diaTrabalhado) ? 1 : 0;
                        employee.DiasFalta += (!diaTrabalhado) ? 1 : 0;
                        if (horasTrabalhadas >= 8) {
                            employee.HorasExtras += horasTrabalhadas - 8;
                        } else {
                            employee.HorasDebito += 8 - horasTrabalhadas;
                        }
                    } else {
                        if (horasTrabalhadas >= 8) {
                            employee.DiasExtras += 1;
                            employee.HorasExtras += horasTrabalhadas - 8;
                        } else {
                            employee.HorasExtras += 8 - horasTrabalhadas;
                        }
                    }

                    dataAtual = dataAtual.AddDays(1);
                }
                var totalDiasTrabalhados = employee.DiasTrabalhados * 8 * HourlyRate;
                var totalDiasExtras = employee.DiasExtras * 8 * HourlyRate;
                var totalDiasFalta = employee.DiasFalta * 8 * HourlyRate;

                var totalHorasExtras = employee.HorasExtras * HourlyRate;
                var totalHorasDebito = employee.HorasDebito * HourlyRate;

                employee.TotalReceber = double.Round((totalDiasTrabalhados + totalDiasExtras + totalHorasExtras - totalDiasFalta - totalHorasDebito), 2, MidpointRounding.AwayFromZero);
                department.TotalDescontos += double.Round(totalDiasFalta + totalHorasDebito, 2, MidpointRounding.AwayFromZero);
                department.TotalExtras += double.Round(totalDiasExtras + totalHorasExtras, 2, MidpointRounding.AwayFromZero);
                department.TotalPagar += double.Round(totalDiasTrabalhados + totalDiasExtras + totalHorasExtras, 2, MidpointRounding.AwayFromZero);
            }
        }
        
        return Ok(JsonSerializer.Serialize(departments));
    }
}