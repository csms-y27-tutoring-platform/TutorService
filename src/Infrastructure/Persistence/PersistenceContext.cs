using Application.Models;
using Microsoft.EntityFrameworkCore;
using TutorService.Application.Models;

namespace Infrastructure.Persistence;

public class PersistenceContext : DbContext
{
    public PersistenceContext(DbContextOptions<PersistenceContext> options, DbSet<Application.Models.Tutor> tutors, DbSet<Subject> subjects, DbSet<TeachingSubject> teachingSubjects, DbSet<ScheduleSlot> scheduleSlots)
        : base(options)
    {
        Tutors = tutors;
        Subjects = subjects;
        TeachingSubjects = teachingSubjects;
        ScheduleSlots = scheduleSlots;
    }

    public DbSet<Application.Models.Tutor> Tutors { get; set; }

    public DbSet<Subject> Subjects { get; set; }

    public DbSet<TeachingSubject> TeachingSubjects { get; set; }

    public DbSet<ScheduleSlot> ScheduleSlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Application.Models.Tutor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property<object>(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property<object>(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property<object>(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property<object>(e => e.Phone ?? string.Empty).HasMaxLength(20);
            entity.Property<object>(e => e.Description ?? string.Empty).HasMaxLength(1000);
            entity.Property<object>(e => e.Status).IsRequired();
            entity.Property<object>(e => e.PreferredFormat).IsRequired();
            entity.Property<object>(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<TeachingSubject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PricePerHour).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExperienceYears).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => new { e.TutorId, e.SubjectId }).IsUnique();
        });

        modelBuilder.Entity<ScheduleSlot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => new { e.TutorId, e.StartTime }).IsUnique();
            entity.HasIndex(e => e.TutorId);
            entity.HasIndex(e => e.Status);
        });
    }
}