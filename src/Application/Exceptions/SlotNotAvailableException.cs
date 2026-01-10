namespace Application.Exceptions;

public class SlotNotAvailableException : Exception
{
    public SlotNotAvailableException(Guid slotId)
        : base($"Schedule slot with ID '{slotId}' is not available for booking.")
    {
        SlotId = slotId;
    }

    public Guid SlotId { get; }
}