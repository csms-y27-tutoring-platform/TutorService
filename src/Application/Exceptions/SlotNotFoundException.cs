namespace Application.Exceptions;

public class SlotNotFoundException : Exception
{
    public SlotNotFoundException(Guid slotId)
        : base($"Schedule slot with ID '{slotId}' was not found.")
    {
        SlotId = slotId;
    }

    public Guid SlotId { get; }
}