using Application.Abstractions;
using Application.Models;
using Infrastructure.Persistence.Database;
using Infrastructure.Persistence.Database.Queries;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

public class ScheduleSlotRepository : IScheduleSlotRepository
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IScheduleSlotQueries _queries;

    public ScheduleSlotRepository(IDatabaseConnectionFactory connectionFactory, IScheduleSlotQueries queries)
    {
        _connectionFactory = connectionFactory;
        _queries = queries;
    }

    public async Task<ScheduleSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _queries.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ScheduleSlot?> GetByTutorAndTimeAsync(Guid tutorId, DateTime startTime, CancellationToken cancellationToken)
    {
        return await _queries.GetByTutorAndTimeAsync(tutorId, startTime, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ScheduleSlot>> GetByTutorIdAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        return await _queries.GetByTutorIdAsync(tutorId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ScheduleSlot>> GetAvailableSlotsAsync(Guid tutorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        return await _queries.GetAvailableSlotsAsync(tutorId, startTime, endTime, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ScheduleSlot> AddAsync(ScheduleSlot slot, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            INSERT INTO schedule_slots (id, tutor_id, start_time, end_time, status, created_at)
            VALUES (@id, @tutorId, @startTime, @endTime, @status, @createdAt)";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", slot.Id);
        command.Parameters.AddWithValue("@tutorId", slot.TutorId);
        command.Parameters.AddWithValue("@startTime", slot.StartTime);
        command.Parameters.AddWithValue("@endTime", slot.EndTime);
        command.Parameters.AddWithValue("@status", (int)slot.Status);
        command.Parameters.AddWithValue("@createdAt", slot.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        return slot;
    }

    public async Task UpdateAsync(ScheduleSlot slot, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            UPDATE schedule_slots 
            SET start_time = @startTime,
                end_time = @endTime,
                status = @status,
                booking_id = @bookingId,
                updated_at = @updatedAt
            WHERE id = @id";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", slot.Id);
        command.Parameters.AddWithValue("@startTime", slot.StartTime);
        command.Parameters.AddWithValue("@endTime", slot.EndTime);
        command.Parameters.AddWithValue("@status", (int)slot.Status);
        command.Parameters.AddWithValue("@bookingId", slot.BookingId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _queries.ExistsAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> IsSlotAvailableAsync(Guid slotId, CancellationToken cancellationToken)
    {
        return await _queries.IsSlotAvailableAsync(slotId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ScheduleSlot?> GetByIdWithTutorAsync(Guid slotId, CancellationToken cancellationToken)
    {
        return await _queries.GetByIdWithTutorAsync(slotId, cancellationToken).ConfigureAwait(false);
    }
}