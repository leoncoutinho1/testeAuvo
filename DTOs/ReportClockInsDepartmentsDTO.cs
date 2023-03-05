namespace testeAuvo.DTOs;

public class ReportClockInsDepartmentsDTO
{
    public ICollection<DepartmentDTO> Departamentos { get; set; }
}

public class DepartmentDTO {
    public string Departamento { get; set; }
    public string MesVigencia { get; set; }
    public int AnoVigencia { get; set; }
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