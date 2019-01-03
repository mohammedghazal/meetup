namespace DatingApp.API.Models
{
    public class Report
    {
        public int ReporterId { get; set; }
        public int ReporteeId { get; set; }
        public User Reporter { get; set; }
        public User Reportee { get; set; }
    }
}