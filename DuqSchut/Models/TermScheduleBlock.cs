namespace DuqSchut.Models
{
    public class TermScheduleBlock
    {
        public int ID { get; set; }
        public DaysOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int TermID { get; set; }
        public Term Term { get; set; }

    }
}
