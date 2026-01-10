namespace Application.Models;

public class ScheduleSlot
{
    public Guid Id { get; private set; }

    public Guid TutorId { get; private set; }

    public DateTime StartTime { get; private set; }

    public DateTime EndTime { get; private set; }

    public SlotStatus Status { get; private set; }

    public Guid? BookingId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public ScheduleSlot(Guid id, Guid tutorId, DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        Id = id;
        TutorId = tutorId;
        StartTime = startTime;
        EndTime = endTime;
        Status = SlotStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    public void Reserve(Guid bookingId)
    {
        if (Status != SlotStatus.Available)
        {
            throw new InvalidOperationException("Slot is not available for reservation");
        }

        Status = SlotStatus.Reserved;
        BookingId = bookingId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Book()
    {
        if (Status != SlotStatus.Reserved)
        {
            throw new InvalidOperationException("Slot must be reserved before booking");
        }

        Status = SlotStatus.Booked;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Release()
    {
        if (Status == SlotStatus.Available)
        {
            throw new InvalidOperationException("Slot is already available");
        }

        Status = SlotStatus.Available;
        BookingId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUnavailable()
    {
        if (Status == SlotStatus.Booked || Status == SlotStatus.Reserved)
        {
            throw new InvalidOperationException("Cannot mark booked or reserved slot as unavailable");
        }

        Status = SlotStatus.Unavailable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsAvailable()
    {
        if (Status == SlotStatus.Booked || Status == SlotStatus.Reserved)
        {
            throw new InvalidOperationException("Cannot mark booked or reserved slot as available");
        }

        Status = SlotStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAvailable()
    {
        return Status == SlotStatus.Available;
    }

    public bool IsReserved()
    {
        return Status == SlotStatus.Reserved;
    }

    public bool IsBooked()
    {
        return Status == SlotStatus.Booked;
    }
}