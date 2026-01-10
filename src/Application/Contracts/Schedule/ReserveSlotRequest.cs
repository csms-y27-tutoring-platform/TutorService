namespace Application.Contracts.Schedule;

public class ReserveSlotRequest
{
    public required Guid SlotId { get; init; }

    public required Guid BookingId { get; init; }
}