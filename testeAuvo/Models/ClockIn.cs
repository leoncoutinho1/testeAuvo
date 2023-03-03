namespace testeAuvo.Models
{
    public class ClockIn
    {
        private DateTime Date { get; set; }
        private DateTime Entry { get; set; }
        private DateTime ExitLunch { get; set; }
        private DateTime EntryLunch { get; set; }
        private DateTime Exit { get; set; }
        private long EmployeeId { get; set; }
    }
}
