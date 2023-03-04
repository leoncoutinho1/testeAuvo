using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testeAuvo.Models
{
    public class Employee
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public Double HourlyRate { get; set; }
        [ForeignKey("Department")]
        public long DepartmentId { get; set; }
        public Department Department { get; set; }
        public List<ClockIn> ClockIns { get; }
    }
}
