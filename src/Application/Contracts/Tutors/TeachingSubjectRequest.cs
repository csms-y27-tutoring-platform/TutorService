namespace Application.Contracts.Tutors;

public class TeachingSubjectRequest
{
    public required Guid SubjectId { get; init; }

    public required decimal PricePerHour { get; init; }

    public string? Description { get; init; }

    public required int ExperienceYears { get; init; }
}