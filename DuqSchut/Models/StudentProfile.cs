namespace DuqSchut.Models
{
    public class StudentProfile
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int TermID { get; set; }
        public double HoursRemaining { get; set; }
        public Term Term { get; set; }
        public User User { get; set; }
    }
}