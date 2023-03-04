using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testeAuvo.Models
{
    public class ClockIn
    {
        [Key]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Entry { get; set; }
        public DateTime ExitLunch { get; set; }
        public DateTime EntryLunch { get; set; }
        public DateTime Exit { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public Employee  Employee { get; set;}
    }
}
