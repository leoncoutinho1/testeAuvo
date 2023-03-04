namespace testeAuvo.Models
{
    public class Department
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Department(long Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
        public List<Employee> Employees { get; }
    }

    
}
