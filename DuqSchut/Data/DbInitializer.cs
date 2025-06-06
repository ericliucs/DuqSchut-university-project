using System.Globalization;
using DuqSchut.Models;

namespace DuqSchut.Data
{
    /**
     <summary>
      Class that initializes the database if it is empty.
     </summary>
    */
    public static class DbInitializer
    {
        public static void Initialize(DuqSchutContext context)
        {
            if (context.Users.Any()) 
            {
                return; // DB has been seeded
            }

            var adminUser = new User{
                ID = "lipeckya@duq.edu", 
                FirstName = "Alex", 
                LastName = "Lipecky", 
                Role = UserRole.Admin
            };

            var adminUser2 = new User{
                ID = "jacksonj@duq.edu", 
                FirstName = "Jeff", 
                LastName = "Jackson", 
                Role = UserRole.Admin
            };

            var tutorUser = new User{
                ID = "tutor@duq.edu", 
                FirstName = "Tutorfirst", 
                LastName = "Tutorlast", 
                Role = UserRole.Tutor
            };

            var tutor2 = new TutorProfile
            {
                UserID = "user123@duq.edu",
                TermID = 1,
                Approved = true,
                Courses = new List<TutorCourse>
                {
                    new TutorCourse 
                    { 
                        TutorProfileUserID = "user123@duq.edu", 
                        TermID = 1, 
                        Course = "COSC160" 
                    },
                    new TutorCourse 
                    { 
                        TutorProfileUserID = "user123@duq.edu", 
                        TermID = 1, 
                        Course = "COSC220" 
                    }
                },
                RegularSchedule = new List<TutorScheduleBlock>
                {
                    new TutorScheduleBlock
                    {
                        ID = 156,
                        DayOfWeek = DaysOfWeek.Monday,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(11, 0),
                        Location = "College Hall 434",
                        TutorProfileID = 167
                    },
                    new TutorScheduleBlock
                    {
                        ID = 234,
                        DayOfWeek = DaysOfWeek.Wednesday,
                        StartTime = new TimeOnly(13, 0),
                        EndTime = new TimeOnly(15, 0),
                        Location = "College Hall 434",
                        TutorProfileID = 167
                    }
                },
                Exceptions = new List<DateBlock>
                {
                    new DateBlock
                    {
                        Date = new DateOnly(2025, 4, 14),
                        StartTime = new TimeOnly(10, 0),
                        EndTime = new TimeOnly(12, 0),
                        TutorProfileID = 167
                    },
                    new DateBlock
                    {
                        Date = new DateOnly(2025, 4, 14),
                        StartTime = new TimeOnly(13, 0),
                        EndTime = new TimeOnly(15, 0),
                        Location = "College Hall 434",
                        TutorProfileID = 167
                    }
                },
            };

            var userTutor2 = new User 
            { 
                ID = "user123@duq.edu", 
                FirstName = "Tutor1",
                LastName = "Tutor1LastName"
                
            };


            var studentUser1 = new User{
                ID = "bowmanj1@duq.edu", 
                FirstName = "Jessica", 
                LastName = "Bowman", 
                Role = UserRole.Student
            };

            var studentUser2 = new User{
                ID = "student@duq.edu", 
                FirstName = "Studentfirst", 
                LastName = "StudentLast", 
                Role = UserRole.Student
            };

            var term = new Term{
                ID=1,
                Name="Spring 2025",
                StartDate = new DateOnly(2025,1,9),
                EndDate = new DateOnly(2025,7,7),
                TimeIncrement = TermTimeIncrement.HalfHour,
                MaxHoursTuteesAllowed = 2,
                MinTutorWeeklyHours = 12,
                MaxTutorWeeklyHours = 15,
                Published = true
            };

            var term2 = new Term{
                ID=2,
                Name="Fall 2025",
                StartDate = new DateOnly(2025,6,9),
                EndDate = new DateOnly(2025,12,30),
                TimeIncrement = TermTimeIncrement.HalfHour,
                MaxHoursTuteesAllowed = 2,
                MinTutorWeeklyHours = 12,
                MaxTutorWeeklyHours = 15,
                Published = true
            };

            var appointment1 = new Appointment{
                ID = 1,
                TuteeID = "bowmanj1@duq.edu",
                TutorID = "tutor@duq.edu",
                Date = new DateOnly(2025,3,25),
                StartTime = new TimeOnly(2,30),
                EndTime = new TimeOnly(3,00),
                Location = ("College Hall"),
                Course = ("Software Engineering"),
                Purpose = ("Review"),
                TermID = 1
            };

            var appointment2 = new Appointment{
                ID = 2,
                TuteeID = "student@duq.edu",
                TutorID = "tutor@duq.edu",
                Date = new DateOnly(2025,3,26),
                StartTime = new TimeOnly(1,00),
                EndTime = new TimeOnly(1,30),
                Location = ("Student Union"),
                Course = ("Physics I"),
                Purpose = ("Help with homework 4"),
                TermID = 1
            };

            var appointment3 = new Appointment{
                ID = 3,
                TuteeID = "student@duq.edu",
                TutorID = "tutor@duq.edu",
                Date = new DateOnly(2025,3,26),
                StartTime = new TimeOnly(1,00),
                EndTime = new TimeOnly(2,00),
                Location = ("Student Union"),
                Course = ("Physics I"),
                Purpose = ("Help with homework 4"),
                TermID = 1
            };

            var appointment4 = new Appointment{
                ID = 4,
                TuteeID = "student@duq.edu",
                TutorID = "tutor@duq.edu",
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                StartTime = new TimeOnly(1,00),
                EndTime = new TimeOnly(2,00),
                Location = "Student Union",
                Course = "COSC220",
                Purpose = "Help with Exam Prep",
                TermID = 1
            };


            var course = new TermCourse{
                TermID = 1,
                Course = "COSC160",
                Term = term
            };

            var course2 = new TermCourse{
                TermID = 1,
                Course = "MATH101",
                Term = term
            };

            var course3 = new TermCourse{
                TermID = 2,
                Course = "COSC220",
                Term = term2
            };

            var course4 = new TermCourse{
                TermID = 2,
                Course = "MATH102",
                Term = term2
            };

            var course5 = new TermCourse{
                TermID = 1,
                Course = "COSC220",
                Term = term
            };

            var scheduleBlock1 = new TermScheduleBlock{
                ID = 1,
                DayOfWeek = DaysOfWeek.Monday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 1,
                Term = term
            };

            var scheduleBlock2 = new TermScheduleBlock{
                ID = 2,
                DayOfWeek = DaysOfWeek.Tuesday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 1,
                Term = term
            };

            var scheduleBlock3 = new TermScheduleBlock{
                ID = 3,
                DayOfWeek = DaysOfWeek.Wednesday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 1,
                Term = term
            };

            var scheduleBlock4 = new TermScheduleBlock{
                ID = 4,
                DayOfWeek = DaysOfWeek.Thursday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 1,
                Term = term
            };

            var scheduleBlock5 = new TermScheduleBlock{
                ID = 5,
                DayOfWeek = DaysOfWeek.Friday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 1,
                Term = term
            };

            var scheduleBlock6 = new TermScheduleBlock{
                ID = 6,
                DayOfWeek = DaysOfWeek.Monday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 2,
                Term = term2
            };

            var scheduleBlock7 = new TermScheduleBlock{
                ID =7,
                DayOfWeek = DaysOfWeek.Tuesday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 2,
                Term = term2
            };

            var scheduleBlock8 = new TermScheduleBlock{
                ID = 8,
                DayOfWeek = DaysOfWeek.Wednesday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 2,
                Term = term2
            };

            var scheduleBlock9 = new TermScheduleBlock{
                ID = 9,
                DayOfWeek = DaysOfWeek.Thursday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 2,
                Term = term2
            };

            var scheduleBlock10 = new TermScheduleBlock{
                ID = 10,
                DayOfWeek = DaysOfWeek.Friday,
                StartTime = new TimeOnly(9,0),
                EndTime = new TimeOnly(20,0),
                TermID = 2,
                Term = term2
            };

            var emailCancellationTemplateStudent = new EmailTemplate{
                Name = "Appointment Cancellation Email Template for Students",
                Subject = "Student Appointment Cancellation",
                Body = "Hello {tutee},\n\n" + 
                    "Your appointment with {tutor} on {date} from {startTime} to {endTime} has been cancelled.\n\n" +
                    "Thank you,\nDuqSchut",
                Type=TemplateType.AppointmentCancellationStudent
            };

            var emailCancellationTemplateTutor = new EmailTemplate{
                Name = "Appointment Cancellation Email template for tutors",
                Subject = "Tutor Appointment Cancellation",
                Body = "Hello {tutor},\n\n" + 
                    "Your appointment with {tutee} on {date} from {startTime} to {endTime} has been cancelled.\n\n" +
                    "We apologize for any inconvenience this may cause.\n\n" +
                    "Thank you,\nDuqSchut",
                Type=TemplateType.AppointmentCancellationTutor
            };

            var emailTutorConfirmationTemplateTutor = new EmailTemplate
            {
                Name = "Tutor Confirmation Template",
                Subject = "Tutor Appointment Confirmation",
                Body = "Hello {tutor}, \n\n" +
                    "You have an upcoming appointment with {tutee} on {date} from {startTime} to {endTime}.\n" +
                    "Location: {location} \n Course: {course} \n Purpose: {purpose} \n Thank You! \n DuqSchut",
                Type=TemplateType.AppointmentConfirmationTutor
            };

            var emailConfirmationTemplateStudent = new EmailTemplate
            {
                Name = "Student Confirmation Template",
                Subject = "Student Appointment Confirmation",
                Body = "Hello {tutee}, \n\n" +
                    "You have an upcoming appointment with {tutor} on {date} from {startTime} to {endTime}.\n" +
                    "Location: {location} \n Course: {course} \n Purpose: {purpose} \n Thank You! \n DuqSchut",
                Type=TemplateType.AppointmentConfirmationStudent
            };

            var emailTutorReminderTemplateTutor = new EmailTemplate
            {
                Name = "Tutor Reminder Template",
                Subject = "Tutor Appointment Reminder",
                Body = "This is an automated reminder of your upcoming appointment with {tutee} on {date} from " +
                       "{startTime} to {endTime}.\n" +
                       "Location: {location} \n Course: {course} \n Purpose: {purpose} \n Thank You! \n DuqSchut",
                Type = TemplateType.AppointmentReminderTutor
            };

            var emailReminderTemplateStudent = new EmailTemplate
            {
                Name = "Student Reminder Template",
                Subject = "Student Appointment Reminder",
                Body = "This is an automated reminder of your upcoming appointment with {tutor} on {date} from " +
                       "{startTime} to {endTime}.\n" +
                       "Location: {location} \n Course: {course} \n Purpose: {purpose} \n Thank You! \n DuqSchut",
                Type = TemplateType.AppointmentReminderStudent
            };

            var studentProfile = new StudentProfile
            {
                UserID = studentUser1.ID,
                TermID = term.ID,
                HoursRemaining = 2
            };

            var studentProfile2 = new StudentProfile
            {
                UserID = studentUser2.ID,
                TermID = term.ID,
                HoursRemaining = 3
            };

            var newTerm = new Term
            {
                ID = 3,
                StartDate = new DateOnly(2023, 9, 1),
                EndDate = new DateOnly(2023, 12, 31),
                Courses = new List<TermCourse>()
            };

            var studentUser3 = new User
            {
                ID = "student3@duq.edu",
                FirstName = "Student",
                LastName = "Three",
                Role = UserRole.Student,
            };

            var studentProfile3 = new StudentProfile
            {
                UserID = studentUser3.ID,
                TermID = newTerm.ID,
                HoursRemaining = 3
            };

            context.Add(newTerm);
            context.Add(studentUser3);
            context.Add(studentProfile3);

            context.Add(adminUser);
            context.Add(adminUser2);
            context.Add(tutorUser);
            context.Add(tutor2);
            context.Add(userTutor2);
            context.Add(studentUser1);
            context.Add(studentUser2);
            context.Add(term);
            context.Add(term2);
            context.Add(appointment1);
            context.Add(appointment2);
            context.Add(appointment3);
            context.Add(appointment4);
            context.Add(course);
            context.Add(course2);
            context.Add(course3);
            context.Add(course4);
            context.Add(course5);
            context.Add(scheduleBlock1);
            context.Add(scheduleBlock2);
            context.Add(scheduleBlock3);
            context.Add(scheduleBlock4);
            context.Add(scheduleBlock5);
            context.Add(scheduleBlock6);
            context.Add(scheduleBlock7);
            context.Add(scheduleBlock8);
            context.Add(scheduleBlock9);
            context.Add(scheduleBlock10);
            context.Add(emailCancellationTemplateStudent);
            context.Add(emailTutorConfirmationTemplateTutor);
            context.Add(emailTutorReminderTemplateTutor);
            context.Add(emailCancellationTemplateTutor);
            context.Add(emailReminderTemplateStudent);
            context.Add(emailConfirmationTemplateStudent);
            context.Add(studentProfile);
            context.Add(studentProfile2);
            context.SaveChanges();
        }
    }
}