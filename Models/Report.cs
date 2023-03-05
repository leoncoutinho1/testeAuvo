using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testeAuvo.Models;

public class Report
{
    public ICollection<DepartmentDTO> Departamentos { get; set; }
    [DisplayName("Mês")]
    [Required(ErrorMessage = "Campo Mês é obrigatório")]
    public byte Mes { get; set; }
        
    [DisplayName("Ano")]
    [Required(ErrorMessage = "Campo Ano é obrigatório")]
    public ushort Ano { get; set; }
}

public class DepartmentDTO {
    public string Departamento { get; set; }

    [DisplayName("Mês")]
    [Required(ErrorMessage = "Campo Mês é obrigatório")]
    public byte MesVigencia { get; set; }
        
    [DisplayName("Ano")]
    [Required(ErrorMessage = "Campo Ano é obrigatório")]
    public ushort AnoVigencia { get; set; }
    public double TotalPagar { get; set; }
    public double TotalDescontos { get; set; }
    public double TotalExtras { get; set; }
    public ICollection<EmployeeDTO> Funcionarios { get; set; }
}

public class EmployeeDTO {
    public string Nome { get; set; }
    public long Codigo { get; set; }
    public double TotalReceber { get; set; }
    public double HorasExtras { get; set; }
    public double HorasDebito { get; set; }
    public int DiasFalta { get; set; }
    public int DiasExtras { get; set; }
    public int DiasTrabalhados { get; set; }
}