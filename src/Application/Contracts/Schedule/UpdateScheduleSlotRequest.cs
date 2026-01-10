namespace Application.Contracts.Schedule;

public class UpdateScheduleSlotRequest
{
    public required DateTime StartTime { get; init; }

    public required DateTime EndTime { get; init; }
}