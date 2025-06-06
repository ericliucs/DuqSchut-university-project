using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DuqSchut.Models;

namespace DuqSchut.Data
{
    public class DuqSchutContext : DbContext
    {
        public DuqSchutContext (DbContextOptions<DuqSchutContext> options)
            : base(options)
        {
        }

        public DbSet<DuqSchut.Models.Term> Terms { get; set; } 
        public DbSet<DuqSchut.Models.User> Users { get; set; } 
        public DbSet<DuqSchut.Models.Appointment> Appointments { get; set; } 
        public DbSet<DuqSchut.Models.TutorProfile> TutorProfiles { get; set; } 
        public DbSet<DuqSchut.Models.TermScheduleBlock> TermScheduleBlocks { get; set; } 
        public DbSet<DuqSchut.Models.TutorScheduleBlock> TutorScheduleBlocks { get; set; } 
        public DbSet<DuqSchut.Models.DateBlock> DateBlocks { get; set; } 
        public DbSet<DuqSchut.Models.TermCourse> TermCourses { get; set; } 
        public DbSet<DuqSchut.Models.TutorCourse> TutorCourses { get; set; }
        public DbSet<DuqSchut.Models.EmailTemplate> EmailTemplates { get; set; }
        public DbSet<DuqSchut.Models.StudentProfile> StudentProfile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Term>().ToTable("Term");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
            modelBuilder.Entity<TutorProfile>().ToTable("TutorProfile");
            modelBuilder.Entity<TermScheduleBlock>().ToTable("TermScheduleBlock");
            modelBuilder.Entity<TutorScheduleBlock>().ToTable("TutorScheduleBlock");
            modelBuilder.Entity<DateBlock>().ToTable("DateBlock");
            modelBuilder.Entity<TermCourse>().ToTable("TermCourse");
            modelBuilder.Entity<TutorCourse>().ToTable("TutorCourse");
            modelBuilder.Entity<EmailTemplate>().ToTable("EmailTemplate");
            modelBuilder.Entity<StudentProfile>().ToTable("StudentProfile");

            // aspnet-codegenerator had trouble using annotations to create the following foreign keys.
            // Based on Copilot-generated code.
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Tutor)
                .WithMany(u => u.AppointmentsAsTutor)
                .HasForeignKey(a => a.TutorID)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Tutee)
                .WithMany(u => u.AppointmentsAsTutee)
                .HasForeignKey(a => a.TuteeID)
                .OnDelete(DeleteBehavior.Restrict); 

            // Code for converting enums back and forth with corresponding strings.
            // See https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations#configuring-a-value-converter
            modelBuilder.Entity<Term>()
                .Property(t => t.TimeIncrement)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<TermTimeIncrement>(v));

            modelBuilder.Entity<TermScheduleBlock>()
                .Property(t => t.DayOfWeek)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<DaysOfWeek>(v));

            modelBuilder.Entity<TutorScheduleBlock>()
                .Property(t => t.DayOfWeek)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<DaysOfWeek>(v));

        }
    }
}
