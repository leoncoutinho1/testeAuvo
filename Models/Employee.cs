using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testeAuvo.Models;
public class Employee {
    [Key]
    public long Id {get; set;}
    [DisplayName("Nome")]
    [Required(ErrorMessage = "Campo Nome é obrigatório")]
    public string Name { get; set; }
    [DisplayName("Valor Hora")]
    [Required(ErrorMessage = "Campo Valor Hora é obrigatório")]
    public double HourlyRate { get; set; }
    [ForeignKey("Departamento")]
    [Required(ErrorMessage = "Campo Departamento é obrigatório")]
    public long DepartmentId { get; set; }
    public Department? Department { get; set; }
    public ICollection<ClockIn>? ClockIns { get; set; }
}