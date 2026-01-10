namespace Application.Contracts.Tutors;

public class GetTutorResponse
{
    public required Guid Id { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public string? Phone { get; init; }

    public string? Description { get; init; }

    public required Models.TutorStatus Status { get; init; }

    public required Models.LessonFormat PreferredFormat { get; init; }

    public int? AverageLessonDurationMinutes { get; init; }

    public IReadOnlyCollection<TeachingSubjectResponse> TeachingSubjects { get; init; } = new List<TeachingSubjectResponse>();

    public required DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}