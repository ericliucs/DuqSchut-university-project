using DuqSchut.Models;
using DuqSchut.Data;
using Microsoft.EntityFrameworkCore;

/* This service class was written entirely by ChatGPT 
    where [Caylah] instructed it to write a shared page based on the @code {...}
    others had written in the tutor and student Index pages */

namespace DuqSchut.Services
{
    public interface IAppointmentService
    {
        Task<IQueryable<Appointment>> GetAppointmentsForUserAsync(string role, string userId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IDbContextFactory<DuqSchutContext> _dbFactory;

        public AppointmentService(
            IDbContextFactory<DuqSchutContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<IQueryable<Appointment>> 
            GetAppointmentsForUserAsync(string role, string userId)
        {
            // Produce IQuerryable data collection of the current user's appointments
            // to be used in the QuickGrid table.
            using var context = _dbFactory.CreateDbContext();

            var user = await context.Users
                .Include(u => u.AppointmentsAsTutor)
                .Include(u => u.AppointmentsAsTutee)
                .SingleAsync(u => u.ID == userId);

            if (user == null) 
            {
                return Enumerable.Empty<Appointment>().AsQueryable();
            }

            return role switch 
            {
                "Tutor" => user.AppointmentsAsTutor.AsQueryable(),
                "Student" => user.AppointmentsAsTutee.AsQueryable(),
                _ => Enumerable.Empty<Appointment>().AsQueryable()
            };
        }
    }
}