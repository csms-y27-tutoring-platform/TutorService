namespace Application.Contracts.Subjects;

public class TeachingSubjectDto
{
    public required Guid Id { get; init; }

    public required Guid TutorId { get; init; }

    public required Guid SubjectId { get; init; }

    public required decimal PricePerHour { get; init; }

    public string? Description { get; init; }

    public required int ExperienceYears { get; init; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}