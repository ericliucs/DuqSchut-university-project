using System.ComponentModel.DataAnnotations;

namespace DuqSchut.Models
{
    public class TutorProfile
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int TermID { get; set; }
        public bool Approved { get; set; }
        public string AboutMe { get; set; }
        public ICollection<TutorCourse> Courses { get; set; }
        public ICollection<TutorScheduleBlock> RegularSchedule { get; set; }
        public ICollection<DateBlock> Exceptions { get; set; }
        public Term Term { get; set; }
        public User User { get; set; }

        #nullable enable
        public byte[]? UserPhoto {get; set;}
        #nullable disable

    }
}