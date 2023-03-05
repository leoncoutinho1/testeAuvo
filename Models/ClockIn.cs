using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testeAuvo.Models;

public class ClockIn
{
    [Key]
    public long Id { get; set; }
    [DisplayName("Entrada")]
    [Required(ErrorMessage = "Campo Entrada é obrigatório")]
    public DateTime Entry { get; set; }

    [DisplayName("Saída")]
    [Required(ErrorMessage = "Campo Saída é obrigatório")]
    public DateTime Exit { get; set; }

    [DisplayName("Ida para o almoço")]
    [Required(ErrorMessage = "Campo Ida para o almoço é obrigatório")]
    public DateTime EntryLunch { get; set; }

    [DisplayName("Volta do almoço")]
    [Required(ErrorMessage = "Campo Volta do almoço é obrigatório")]
    public DateTime ExitLunch { get; set; }
   
    [Required(ErrorMessage = "Campo Funcionário é obrigatório")]
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    [DisplayName("Funcionário")]
    public Employee? Employee { get; set; }
}
