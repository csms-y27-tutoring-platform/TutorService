using Application.Models;
using Npgsql;

namespace Infrastructure.Persistence.Database.Queries;

public class NpgsqlScheduleSlotQueries : IScheduleSlotQueries
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public NpgsqlScheduleSlotQueries(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ScheduleSlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT id, tutor_id, start_time, end_time, status, booking_id, created_at, updated_at
            FROM schedule_slots
            WHERE id = @id";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return MapScheduleSlot(reader);
        }

        return null;
    }

    public async Task<ScheduleSlot?> GetByTutorAndTimeAsync(Guid tutorId, DateTime startTime, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT id, tutor_id, start_time, end_time, status, booking_id, created_at, updated_at
            FROM schedule_slots
            WHERE tutor_id = @tutorId AND start_time = @startTime";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tutorId", tutorId);
        command.Parameters.AddWithValue("@startTime", NpgsqlTypes.NpgsqlDbType.Timestamp, startTime);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return MapScheduleSlot(reader);
        }

        return null;
    }

    public async Task<IReadOnlyCollection<ScheduleSlot>> GetByTutorIdAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT id, tutor_id, start_time, end_time, status, booking_id, created_at, updated_at
            FROM schedule_slots
            WHERE tutor_id = @tutorId
            ORDER BY start_time";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tutorId", tutorId);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var slots = new List<ScheduleSlot>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            slots.Add(MapScheduleSlot(reader));
        }

        return slots;
    }

    public async Task<IReadOnlyCollection<ScheduleSlot>> GetAvailableSlotsAsync(Guid tutorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT id, tutor_id, start_time, end_time, status, booking_id, created_at, updated_at
            FROM schedule_slots
            WHERE tutor_id = @tutorId 
            AND start_time >= @startTime 
            AND end_time <= @endTime
            AND status = @availableStatus
            ORDER BY start_time";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tutorId", tutorId);
        command.Parameters.AddWithValue("@startTime", NpgsqlTypes.NpgsqlDbType.Timestamp, startTime);
        command.Parameters.AddWithValue("@endTime", NpgsqlTypes.NpgsqlDbType.Timestamp, endTime);
        command.Parameters.AddWithValue("@availableStatus", (int)SlotStatus.Available);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var slots = new List<ScheduleSlot>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            slots.Add(MapScheduleSlot(reader));
        }

        return slots;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT 1 FROM schedule_slots WHERE id = @id";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<bool> IsSlotAvailableAsync(Guid slotId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT 1 FROM schedule_slots 
            WHERE id = @slotId AND status = @availableStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@slotId", slotId);
        command.Parameters.AddWithValue("@availableStatus", (int)SlotStatus.Available);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<ScheduleSlot?> GetByIdWithTutorAsync(Guid slotId, CancellationToken cancellationToken)
    {
        return await GetByIdAsync(slotId, cancellationToken).ConfigureAwait(false);
    }

    private static ScheduleSlot MapScheduleSlot(NpgsqlDataReader reader)
    {
        var slot = new ScheduleSlot(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetDateTime(2),
            reader.GetDateTime(3));

        var status = (SlotStatus)reader.GetInt32(4);
        if (status == SlotStatus.Reserved)
        {
            Guid? bookingId = reader.IsDBNull(5) ? null : reader.GetGuid(5);
            if (bookingId.HasValue) slot.Reserve(bookingId.Value);
        }

        return slot;
    }
}