using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace testeAuvo.Models;

public class Department
{
    [Key]
    public long Id { get; set; }
    [DisplayName("Valor Hora")]
    [Required(ErrorMessage = "Campo Nome é obrigatório")]
    public string Name { get; set; }
    public ICollection<Employee>? Employees { get; set; }
}
