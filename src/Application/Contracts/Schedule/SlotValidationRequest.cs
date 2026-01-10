namespace Application.Contracts.Schedule;

public class SlotValidationRequest
{
    public required Guid SlotId { get; init; }

    public required Guid TutorId { get; init; }

    public required Guid SubjectId { get; init; }
}