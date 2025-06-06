using Microsoft.EntityFrameworkCore;

namespace DuqSchut.Models
{
    /**
     <summary>
      Class for representing the multivalued Courses attribute of the
      Term entity.
     </summary>
    */
    [PrimaryKey(nameof(TermID), nameof(Course))]
    public class TermCourse
    {
        // public int ID { get; set; }
        public int TermID { get; set; }
        public string Course { get; set; }

        public Term Term { get; set; }
    }
}