using System.ComponentModel.DataAnnotations.Schema;

namespace DuqSchut.Models
{
    public class Appointment
    {
        public int ID { get; set; }
        public string TuteeID { get; set; }
        public string TutorID { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Location { get; set; }
        public string Course { get; set; }
        public string Purpose { get; set; }
        public int TermID { get; set; }

        public Term Term { get; set; }
        // See DuqSchutContext.cs for foreign key mapping
        public User Tutee { get; set; }
        // See DuqSchutContext.cs for foreign key mapping
        public User Tutor { get; set; }
    }
}