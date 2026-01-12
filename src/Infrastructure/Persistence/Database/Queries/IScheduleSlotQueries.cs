using Application.Models;

namespace Infrastructure.Persistence.Database.Queries;

public interface IScheduleSlotQueries
{
    public Task<ScheduleSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<ScheduleSlot?> GetByTutorAndTimeAsync(Guid tutorId, DateTime startTime, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<ScheduleSlot>> GetByTutorIdAsync(Guid tutorId, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<ScheduleSlot>> GetAvailableSlotsAsync(Guid tutorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    public Task<bool> IsSlotAvailableAsync(Guid slotId, CancellationToken cancellationToken);

    public Task<ScheduleSlot?> GetByIdWithTutorAsync(Guid slotId, CancellationToken cancellationToken);
}