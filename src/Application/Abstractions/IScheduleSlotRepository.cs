using Application.Models;

namespace Application.Abstractions;

public interface IScheduleSlotRepository
{
    public Task<ScheduleSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<ScheduleSlot?> GetByTutorAndTimeAsync(Guid tutorId, DateTime startTime, CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<ScheduleSlot>> GetByTutorIdAsync(Guid tutorId, CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<ScheduleSlot>> GetAvailableSlotsAsync(Guid tutorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

    public Task<ScheduleSlot> AddAsync(ScheduleSlot slot, CancellationToken cancellationToken = default);

    public Task UpdateAsync(ScheduleSlot slot, CancellationToken cancellationToken = default);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<bool> IsSlotAvailableAsync(Guid slotId, CancellationToken cancellationToken = default);

    public Task<ScheduleSlot?> GetByIdWithTutorAsync(Guid slotId, CancellationToken cancellationToken = default);
}