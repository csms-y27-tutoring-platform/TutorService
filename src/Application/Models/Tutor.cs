using Google.Protobuf.WellKnownTypes;

namespace Application.Models;

public class Tutor
{
    public Guid Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Description { get; private set; }

    public TutorStatus Status { get; private set; }

    public LessonFormat PreferredFormat { get; private set; }

    public int? AverageLessonDurationMinutes { get; private set; }

    public ICollection<TeachingSubject> TeachingSubjects { get; private set; }

    public Timestamp CreatedAt { get; private set; }

    public Timestamp UpdatedAt { get; private set; }

    public Tutor(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? phone,
        string? description,
        LessonFormat preferredFormat,
        int? averageLessonDurationMinutes,
        Timestamp updatedAt)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Description = description;
        Status = TutorStatus.Active;
        PreferredFormat = preferredFormat;
        AverageLessonDurationMinutes = averageLessonDurationMinutes;
        UpdatedAt = updatedAt;
        TeachingSubjects = new List<TeachingSubject>();
        CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow);
    }

    public void Update(
        string firstName,
        string lastName,
        string? phone,
        string? description,
        LessonFormat preferredFormat,
        int? averageLessonDurationMinutes)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Description = description;
        PreferredFormat = preferredFormat;
        AverageLessonDurationMinutes = averageLessonDurationMinutes;
        UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow);
    }

    public void Deactivate()
    {
        Status = TutorStatus.Inactive;
        UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow);
    }

    public void Activate()
    {
        Status = TutorStatus.Active;
        UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow);
    }

    public void Suspend()
    {
        Status = TutorStatus.Suspended;
        UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow);
    }

    public void AddTeachingSubject(TeachingSubject subject)
    {
        if (TeachingSubjects.Any(s => s.SubjectId == subject.SubjectId))
        {
            throw new InvalidOperationException("Teaching subject already exists");
        }

        TeachingSubjects.Add(subject);
    }

    public void RemoveTeachingSubject(Guid subjectId)
    {
        TeachingSubject? subject = TeachingSubjects.FirstOrDefault(s => s.SubjectId == subjectId);
        if (subject != null)
        {
            TeachingSubjects.Remove(subject);
        }
    }

    public TeachingSubject? GetTeachingSubject(Guid subjectId)
    {
        return TeachingSubjects.FirstOrDefault(s => s.SubjectId == subjectId);
    }

    public string FullName => $"{FirstName} {LastName}";

    public bool IsActive => Status == TutorStatus.Active;
}