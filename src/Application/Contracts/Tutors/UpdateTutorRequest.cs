namespace Application.Contracts.Tutors;

public class UpdateTutorRequest
{
    public required Guid TutorId { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public string? Phone { get; init; }

    public string? Description { get; init; }

    public Models.LessonFormat PreferredFormat { get; init; }

    public int? AverageLessonDurationMinutes { get; init; }

    public ICollection<TeachingSubjectRequest> TeachingSubjects { get; init; } = new List<TeachingSubjectRequest>();
}