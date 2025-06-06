namespace DuqSchut.Models
{
    public class DateBlock
    {
        public int ID { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Location { get; set; }
        public int TutorProfileID { get; set; }
    }
}
