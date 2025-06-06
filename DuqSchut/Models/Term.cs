/** Modifications to this file were making the DateOnly objects nullable / unknown
 This lets us do some extra modifications to the DateOnly objects and adds functionality.
 adding ? after a object makes the object nullable or a value can be unknown to start.
 This fixes the issue of our admin having to scroll from Jan,01,0001 to jan,01,9999

 Also added the nullable to the time increment. This is due to the webpage generating an
 error when the page is autopopulated with hour.
 It then makes the user select halfhour then hour again to correctly populate the database.
 This above issue is now fixed.

 - Joshua Stenger
**/
namespace DuqSchut.Models
{
    public class Term
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public TermTimeIncrement? TimeIncrement { get; set; }
        public decimal MaxHoursTuteesAllowed { get; set; }
        public decimal MinTutorWeeklyHours { get; set; }
        public decimal MaxTutorWeeklyHours { get; set; }
        public bool Published { get; set; }

        public ICollection<TermCourse> Courses { get; set; }
        public ICollection<TermScheduleBlock> TutoringSchedule { get; set; }
        public ICollection<TutorProfile> TutorProfiles { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}