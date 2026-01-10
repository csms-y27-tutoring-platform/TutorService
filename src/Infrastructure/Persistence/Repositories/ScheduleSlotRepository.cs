using Application.Abstractions;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence.Repositories;

public class ScheduleSlotRepository : IScheduleSlotRepository
{
    private readonly PersistenceContext _context;

    public ScheduleSlotRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<ScheduleSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleSlots
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<ScheduleSlot?> GetByTutorAndTimeAsync(Guid tutorId, DateTime startTime, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleSlots
            .FirstOrDefaultAsync(s => s.TutorId == tutorId && s.StartTime == startTime, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ScheduleSlot>> GetByTutorIdAsync(Guid tutorId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleSlots
            .Where(s => s.TutorId == tutorId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ScheduleSlot>> GetAvailableSlotsAsync(Guid tutorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleSlots
            .Where(s => s.TutorId == tutorId
                && s.StartTime >= startTime
                && s.EndTime <= endTime
                && s.Status == SlotStatus.Available)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<ScheduleSlot> AddAsync(ScheduleSlot slot, CancellationToken cancellationToken = default)
    {
        EntityEntry<ScheduleSlot> entry = await _context.ScheduleSlots.AddAsync(slot, cancellationToken)
            .ConfigureAwait(false);
        return entry.Entity;
    }

    public async Task UpdateAsync(ScheduleSlot slot, CancellationToken cancellationToken = default)
    {
        _context.ScheduleSlots.Update(slot);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleSlots
            .AnyAsync(s => s.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> IsSlotAvailableAsync(Guid slotId, CancellationToken cancellationToken = default)
    {
        ScheduleSlot? slot = await _context.ScheduleSlots
            .FirstOrDefaultAsync(s => s.Id == slotId, cancellationToken)
            .ConfigureAwait(false);

        return slot?.Status == SlotStatus.Available;
    }

    public async Task<ScheduleSlot?> GetByIdWithTutorAsync(Guid slotId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleSlots
            .Include(s => s.TutorId)
            .FirstOrDefaultAsync(s => s.Id == slotId, cancellationToken)
            .ConfigureAwait(false);
    }
}