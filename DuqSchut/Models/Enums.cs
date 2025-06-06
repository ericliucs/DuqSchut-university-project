using System.ComponentModel.DataAnnotations;
namespace DuqSchut.Models
{
    // Enums need special handling to store in the database.
    // See code in DuqSchutContext.cs.
    public enum TermTimeIncrement
    {
        Hour,
        HalfHour
    }
    public enum DaysOfWeek
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }
    public enum UserRole
    {
        Admin,
        Tutor,
        Student
    }
    public enum TemplateType
    {
        AppointmentCancellationStudent,
        AppointmentCancellationTutor,
        AppointmentConfirmationStudent,
        AppointmentConfirmationTutor,
        AppointmentReminderStudent,
        AppointmentReminderTutor
    }
}