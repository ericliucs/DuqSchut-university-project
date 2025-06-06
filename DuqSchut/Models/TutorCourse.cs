using Microsoft.EntityFrameworkCore;

namespace DuqSchut.Models
{
    /**
     <summary>
      Class for representing the multivalued Courses attribute of the
      TutorProfile entity.
     </summary>
    */
    [PrimaryKey(nameof(TutorProfileUserID), nameof(TermID), nameof(Course))]
    public class TutorCourse
    {
        // public int ID { get; set; }
        public string TutorProfileUserID { get; set; }
        public int TermID { get; set; }
        public string Course { get; set; }

        public TutorProfile TutorProfile { get; set; }
        public Term Term { get; set; }
    }
}