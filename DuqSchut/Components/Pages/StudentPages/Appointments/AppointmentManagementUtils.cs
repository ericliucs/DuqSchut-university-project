using DuqSchut.Models;
using Radzen;

public static class AppointmentManagementUtils
{
    /**
        <summary>
            This uses makes a List of dates with no available appointments to select.
        </summary>
        <param name="term"> This is the current term for the appointment</param>
        <returns> This returns a list of dates with no appointments available. </returns>
    */
    public static List<DateOnly> GetDisabledDates(Term term)
    {
        DateOnly endDate = term.EndDate ?? DateOnly.MaxValue;
        int difference = (endDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

        var allDates = Enumerable.Range(0, difference)
            .Select(offset => DateOnly.FromDateTime(DateTime.Today.AddDays(offset)))
            .ToList();

        List<DateOnly> disabledDates = new();

        foreach (var date in allDates)
        {
            bool hasAvailableTutor = false;

            foreach (var tutor in term.TutorProfiles)
            {
                var validIntervals = new List<(TimeOnly Start, TimeOnly End)>();

                if (tutor.RegularSchedule.Any(b => b.DayOfWeek.ToString() == date.DayOfWeek.ToString()))
                {
                    foreach (var block in tutor.RegularSchedule.Where(b => b.DayOfWeek.ToString() == date.DayOfWeek.ToString()))
                    {
                        var overlappingException = tutor.Exceptions
                            .FirstOrDefault(exc => exc.Date == date && exc.StartTime >= block.StartTime && exc.StartTime < block.EndTime);

                        TimeOnly intervalEnd = overlappingException?.StartTime ?? block.EndTime;
                        validIntervals.Add((block.StartTime, intervalEnd));
                    }
                }

                validIntervals.AddRange(tutor.Exceptions
                    .Where(exc => exc.Date == date && !string.IsNullOrEmpty(exc.Location))
                    .Select(exc => (exc.StartTime, exc.EndTime)));

                if (validIntervals.Any())
                {
                    hasAvailableTutor = true;
                    break;
                }
            }

            if (!hasAvailableTutor)
            {
                disabledDates.Add(date);
            }
        }

        return disabledDates;
    }

    /**
        <summary>
            This uses makes a List of dates with no available appointments to select.
        </summary>
        <param name="args"> This contains calendar dates</param>
        <param name="term"> This is the current term for the appointment</param>
        <param name="disabledDates"> These are the disabled dates</param>
        <returns> This returns a list of dates with no appointments available. </returns>
    */
    public static void RenderDate(DateRenderEventArgs args, Term term, List<DateOnly> disabledDates)
    {
        DateOnly startDate = term.StartDate ?? DateOnly.MinValue;
        DateOnly endDate = term.EndDate ?? DateOnly.MaxValue;

        bool isDisabled = disabledDates.Contains(DateOnly.FromDateTime(args.Date)) ||
                          args.Date.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday ||
                          DateOnly.FromDateTime(args.Date) < DateOnly.FromDateTime(DateTime.Now) ||
                          DateOnly.FromDateTime(args.Date) > endDate;

        if (!isDisabled)
        {
            args.Attributes.Add("style", "background-color: orange; border-color: white;");
        }

        args.Disabled = isDisabled;
    }



    /**
        <summary>
            This retrieves the updated location for an appointment based on the tutor's schedule. 
            It first checks exceptions with valid locations, then falls back to the tutor's regular schedule.
        </summary>
        <param name="selectedTutor">This is the tutor whose schedule is being checked.</param>
        <param name="newAppointment">This is the appointment with a Date, StartTime, and EndTime.</param>
        <returns>The location for the appointment or an empty string if unavailable.</returns>
    */

    public static string GetUpdatedLocation(User student, TutorProfile selectedTutor, Appointment newAppointment)
    {
        if (selectedTutor.User.AppointmentsAsTutor.Any(existingAppointment =>
                (newAppointment.ID == 0 || existingAppointment.ID != newAppointment.ID) &&
                existingAppointment.Date == newAppointment.Date &&
                (newAppointment.EndTime > existingAppointment.StartTime && 
                newAppointment.EndTime <= existingAppointment.EndTime)) ||
            student.AppointmentsAsTutee.Any(existingAppointment =>
                (newAppointment.ID == 0 || existingAppointment.ID != newAppointment.ID) &&
                existingAppointment.Date == newAppointment.Date &&
                (newAppointment.EndTime > existingAppointment.StartTime && 
                newAppointment.EndTime <= existingAppointment.EndTime)))
        {
            return "";
        }
        
        // This attempts to find an exception in the tutor's schedule that corresponds to the endtime of the appointment
        var exception = selectedTutor.Exceptions
            .Where(exc => exc.Date == newAppointment.Date)
            .FirstOrDefault(exc => (newAppointment.EndTime > exc.StartTime && newAppointment.EndTime < exc.EndTime) || 
                                newAppointment.EndTime == exc.EndTime);

        // If an exception is found, this returns the location associated with it or an empty string if it is null
        if (exception != null)
        {
            return exception.Location ?? "";
        }

        // This attempts to find a regular schedule block in the tutor's schedule that corresponds to the endtime of the appointment
        var matchingBlock = selectedTutor.RegularSchedule
            .Where(block => block.DayOfWeek.ToString() == newAppointment.Date.DayOfWeek.ToString())
            .FirstOrDefault(block => (newAppointment.EndTime > block.StartTime && newAppointment.EndTime < block.EndTime) || 
                                newAppointment.EndTime == block.EndTime);;

        // If an regular schedule block is found, this returns the location associated with it
        if (matchingBlock != null)
        {
            return matchingBlock.Location;
        }

        return ""; // Returns an empty string
    }


    
    
    /**
        <summary>
            This returns the tutor that was selected by the user.
        </summary>
        <param name="term">This is the term for which an appointment is being scheduled.</param>
        <param name="newAppointment">This is the appointment that the user is trying to schedule</param>
        <returns> This returns a list of the TutorProfiles for the tutors associated with the passed-in term.</returns>
    */
    public static List<TutorProfile> GetFilteredTutors(Term term, Appointment newAppointment)
    {
        var tutors = new List<TutorProfile>();

        // This looks through the courses a tutor can teach in the given term and the days of the week the tutor is available
        foreach (var tutor in term.TutorProfiles)
        {
            foreach (var course in tutor.Courses)
            {
                if (course.Course == newAppointment.Course && 
                    (tutor.RegularSchedule.Any(block => block.DayOfWeek.ToString() == newAppointment.Date.DayOfWeek.ToString()) || 
                    tutor.Exceptions.Any(exc => exc.Date == newAppointment.Date && !string.IsNullOrEmpty(exc.Location))))
                {
                    tutors.Add(tutor);
                }

            }
        }

        return tutors;
    }

    /**
        <summary>
            This returns the tutor that was selected by the user.
        </summary>
        <param name="term">This is the term for which an appointment is being scheduled.</param>
        <param name="tutorUserId">This is the tutor's UserID as a string.</param>
        <returns> This returns the TutorProfile of the selected tutor.</returns>
    */
    public static TutorProfile GetSelectedTutor(Term term, string tutorUserId)
    {
        return term.TutorProfiles.FirstOrDefault(tutor => tutorUserId?.Equals(tutor.UserID) == true); // This returns the selected tutor from the term based on the passed in tutorUserId 
    }

    /**
        <summary>
            This determines available start times for an appointment based on the tutor's schedule, exceptions,
            and any existing appointments for the student or the tutor 
        </summary>
        <param name="student">This is the student scheduling the appointment.</param>
        <param name="selectedTutor">This is the tutor whose availability is being checked.</param>
        <param name="newAppointment">This is an appointment with a specific date and requested time.</param>
        <returns>A list of available start times.</returns>
    */
    public static List<TimeOnly> GetStartTimes(User student, TutorProfile selectedTutor, Appointment newAppointment)
    {
        var startTimes = new List<TimeOnly>();

        // This looks through the tutor's regular schedule and exceptions to create a list of ordered starting appointments
        startTimes = selectedTutor.RegularSchedule
        .Where(block => block.DayOfWeek.ToString() == newAppointment.Date.DayOfWeek.ToString())
        .SelectMany(block => Enumerable.Range(0, (int)((block.EndTime - block.StartTime).TotalMinutes / 30))
            .Select(i => block.StartTime.AddMinutes(i * 30)))
        .Where(time => !selectedTutor.Exceptions.Any(exc =>
            exc.Date == newAppointment.Date &&
            time >= exc.StartTime &&
            time < exc.EndTime))
        .Concat(selectedTutor.Exceptions
            .Where(exc => !string.IsNullOrEmpty(exc.Location))
            .SelectMany(exc => Enumerable.Range(0, (int)((exc.EndTime - exc.StartTime).TotalMinutes / 30))
                .Select(i => exc.StartTime.AddMinutes(i * 30))))
        .OrderBy(time => time)
        .ToList();

        // This filters any start times that conflict with already made appointments for the student or tutor
        startTimes = startTimes.Where(time => 
        !selectedTutor.User.AppointmentsAsTutor.Any(existingAppointment =>
            existingAppointment.Date == newAppointment.Date &&
            (time == existingAppointment.StartTime || 
             (time > existingAppointment.StartTime && time < existingAppointment.EndTime))) &&
        !student.AppointmentsAsTutee.Any(existingAppointment =>
            existingAppointment.Date == newAppointment.Date &&
            (time == existingAppointment.StartTime || 
             (time > existingAppointment.StartTime && time < existingAppointment.EndTime))))
        .Distinct()
        .ToList();

        return startTimes;
    }

    /**
        <summary>
            This sets the start time and potential end times of an appointment based on the tutor’s availability.
        </summary>
        <param name="newAppointment">This is the appointment being scheduled.</param>
        <param name="selectedTimeStr">This is the selected start time as a string.</param>
        <param name="tutor">This is the tutor whose availability is checked.</param>
        <param name="firstEndTime">This is the first possible end time (30 minutes).</param>
        <param name="secondEndTime">This is the second possible end time (60 minutes), or default if unavailable.</param>
    */
    public static void SetAppointmentTimes(Appointment newAppointment, string selectedTimeStr, User student, TutorProfile tutor, out TimeOnly firstEndTime, out TimeOnly secondEndTime)
    {
        // If the ToString() of the parameter is converted back into a TimeOnly object, it is set to equal the start time of the appointment,
        // and the endtimes for a half-hour and hour long appointment are calculated
        if (TimeOnly.TryParse(selectedTimeStr, out var selectedTime))
        {
            newAppointment.StartTime = selectedTime;
            firstEndTime = selectedTime.AddMinutes(30);
            newAppointment.EndTime = firstEndTime;

            Appointment appointment =  new Appointment{Date = newAppointment.Date, StartTime = newAppointment.StartTime, EndTime = selectedTime.AddMinutes(60)};
            if (!string.IsNullOrEmpty(GetUpdatedLocation(student, tutor, appointment)))
            {
                secondEndTime = selectedTime.AddMinutes(60);
            }
            else
            {
                secondEndTime = default;
            }
        }
        else
        {
            firstEndTime = default;
            secondEndTime = default;
        }
    }

    /**
        <summary>
            This updates the EndTime for the passed-in appointment.
        </summary>
        <param name="newAppointment">This is the appointment being scheduled.</param>
        <param name="selectedTimeStr">This is the selected end time as a string.</param>
    */
    public static void UpdateEndTime(Appointment newAppointment, string selectedTimeStr)
    {
        // The passed in string is converted back into a TimeOnly object
        if (TimeOnly.TryParse(selectedTimeStr, out var selectedTime))
        {
            newAppointment.EndTime = selectedTime;
        }
    }

    

    /**
        <summary>
            This validates whether an appointment meets all required criteria, including:
            - Required fields (location, course, purpose, end time)
            - Tutor availability
            - Conflicts with existing appointments
        </summary>
        <param name="newAppointment">This is the appointment being validated.</param>
        <param name="student">This is the student requesting the appointment.</param>
        <param name="selectedTutor">This is the tutor associated with the appointment.</param>
        <returns>A string with error messages describing issues with the appointment.</returns>
    */
    public static string ValidateAppointment(Appointment newAppointment, User student, TutorProfile selectedTutor)
    {
        string errorMessage = "";

        // This tells the user what information they neglected to add
        if (string.IsNullOrWhiteSpace(newAppointment.Location) || string.IsNullOrWhiteSpace(newAppointment.Course) || string.IsNullOrWhiteSpace(newAppointment.Purpose) || newAppointment.EndTime == default(TimeOnly))
        {
            if (newAppointment.EndTime==default(TimeOnly))
            {
                errorMessage += "<li style=\"color:red\">End Time has not been set.</li>";
            }

            if (string.IsNullOrWhiteSpace(newAppointment.Location))
            {
                errorMessage += "<li style=\"color:red\">Location has not been set.</li>";
            }

            if(string.IsNullOrWhiteSpace(newAppointment.Course))
            {
                errorMessage += "<li style=\"color:red\">Course has not been set.</li>";
            }

            if(string.IsNullOrWhiteSpace(newAppointment.Purpose))
            {
                errorMessage += "<li style=\"color:red\">Purpose has not been provided.</li>";
            }

            return errorMessage;
        }
        
        var validIntervals = new List<(TimeOnly Start, TimeOnly End)>(); // Intervals of Valid Times

        // Adds intervals in a tutor's regular schedule, excluding any overlapping exceptions, for which an appointment could be made
        if (selectedTutor.RegularSchedule.Any(b => b.DayOfWeek.ToString() == newAppointment.Date.DayOfWeek.ToString()))
        {
            foreach (var block in selectedTutor.RegularSchedule.Where(b => b.DayOfWeek.ToString() == newAppointment.Date.DayOfWeek.ToString()))
            {
                var overlappingException = selectedTutor.Exceptions
                    .Where(exc => exc.Date == newAppointment.Date)
                    .FirstOrDefault(exc => exc.StartTime >= block.StartTime && exc.StartTime < block.EndTime);

                TimeOnly intervalEnd;

                if (overlappingException != null)
                {
                    intervalEnd = overlappingException.StartTime;
                }
                else
                {
                    intervalEnd = block.EndTime;
                }

                validIntervals.Add((block.StartTime, intervalEnd));
            }
        }


        // This adds all start times and end times for exceptions with a non-null location on the date of the appointment 
        validIntervals.AddRange(selectedTutor.Exceptions
            .Where(exc => exc.Date == newAppointment.Date && !string.IsNullOrEmpty(exc.Location))
            .Select(exc => (exc.StartTime, exc.EndTime)));


        // Determines if both the start and end times fit within one of the Intervals
        bool isValidAppointment = validIntervals.Any(interval => 
            newAppointment.StartTime >= interval.Start && newAppointment.StartTime < interval.End) &&
            validIntervals.Any(interval => newAppointment.EndTime > interval.Start && newAppointment.EndTime <= interval.End);

        if (!isValidAppointment)
        {
            errorMessage += "<li style=\"color:red\">The tutor is not available at the selected date and time.</li>";
            return errorMessage;
        }



        // This checks if the student or the tutor have made any appointments that conflict with the selected date and times
        if ((selectedTutor.User.AppointmentsAsTutor != null &&
                selectedTutor.User.AppointmentsAsTutor.Any(existingAppointment =>
                (newAppointment.ID == 0 || existingAppointment.ID != newAppointment.ID) &&
                existingAppointment.Date.CompareTo(newAppointment.Date) == 0 &&
                newAppointment.StartTime >= existingAppointment.StartTime &&
                newAppointment.StartTime < existingAppointment.EndTime &&
                newAppointment.EndTime > existingAppointment.StartTime &&
                newAppointment.EndTime <= existingAppointment.EndTime)) ||

            (student.AppointmentsAsTutee != null &&
                student.AppointmentsAsTutee.Any(existingAppointment =>
                (newAppointment.ID == 0 || existingAppointment.ID != newAppointment.ID) &&
                existingAppointment.Date.CompareTo(newAppointment.Date) == 0 &&
                newAppointment.StartTime >= existingAppointment.StartTime &&
                newAppointment.StartTime < existingAppointment.EndTime &&
                newAppointment.EndTime > existingAppointment.StartTime &&
                newAppointment.EndTime <= existingAppointment.EndTime)))
        {
            errorMessage += "<li style=\"color:red\">The appointment overlaps with another made appointment.</li>";

            return errorMessage;
        }

        return errorMessage;
    }
}
