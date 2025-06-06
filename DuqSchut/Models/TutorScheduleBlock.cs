namespace DuqSchut.Models
{
    public class TutorScheduleBlock
    {
        public int ID { get; set; }
        public DaysOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Location { get; set; }

        public int TutorProfileID { get; set; }
        //The isActive field was added to distinguish between available and unavailable blocks
        public bool isActive { get; set; }
    }
}
