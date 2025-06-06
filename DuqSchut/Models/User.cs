using System.ComponentModel.DataAnnotations.Schema;

namespace DuqSchut.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }

        public ICollection<Appointment> AppointmentsAsTutor { get; set; }
        public ICollection<Appointment> AppointmentsAsTutee { get; set; }
        public ICollection<TutorProfile> TutorProfiles { get; set; }
        public ICollection<StudentProfile> StudentProfiles { get; set; }
    }
}

