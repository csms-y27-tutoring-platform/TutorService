namespace Application.Contracts.Schedule;

public class CreateScheduleSlotRequest
{
    public required Guid TutorId { get; init; }

    public required DateTime StartTime { get; init; }

    public required DateTime EndTime { get; init; }
}