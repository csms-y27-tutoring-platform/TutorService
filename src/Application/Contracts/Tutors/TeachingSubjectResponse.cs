namespace Application.Contracts.Tutors;

public class TeachingSubjectResponse
{
    public required Guid Id { get; init; }

    public required Guid SubjectId { get; init; }

    public required string SubjectName { get; init; }

    public required decimal PricePerHour { get; init; }

    public string? Description { get; init; }

    public required int ExperienceYears { get; init; }
}