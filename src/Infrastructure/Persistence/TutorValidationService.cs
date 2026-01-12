using Application.Abstractions;
using Application.Exceptions;
using Application.Models;
using Infrastructure.Persistence.Database;
using Npgsql;

namespace Infrastructure.Persistence;

public class TutorValidationService : ITutorValidationService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public TutorValidationService(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ValidateTutorExistsAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT 1 FROM tutors WHERE id = @tutorId AND status != @deletedStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tutorId", tutorId);
        command.Parameters.AddWithValue("@deletedStatus", (int)TutorStatus.Deleted);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<bool> ValidateSubjectExistsAsync(Guid subjectId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT 1 FROM subjects WHERE id = @subjectId";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@subjectId", subjectId);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<bool> ValidateTutorTeachesSubjectAsync(Guid tutorId, Guid subjectId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT 1 FROM teaching_subjects ts
            INNER JOIN tutors t ON ts.tutor_id = t.id
            WHERE ts.tutor_id = @tutorId 
            AND ts.subject_id = @subjectId
            AND t.status != @deletedStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tutorId", tutorId);
        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@deletedStatus", (int)TutorStatus.Deleted);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<bool> ValidateSlotAvailabilityAsync(Guid slotId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT 1 FROM schedule_slots ss
            INNER JOIN tutors t ON ss.tutor_id = t.id
            WHERE ss.id = @slotId 
            AND ss.status = @availableStatus
            AND t.status = @activeStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@slotId", slotId);
        command.Parameters.AddWithValue("@availableStatus", (int)SlotStatus.Available);
        command.Parameters.AddWithValue("@activeStatus", (int)TutorStatus.Active);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<decimal> GetLessonPriceAsync(Guid tutorId, Guid subjectId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT ts.price_per_hour 
            FROM teaching_subjects ts
            INNER JOIN tutors t ON ts.tutor_id = t.id
            WHERE ts.tutor_id = @tutorId 
            AND ts.subject_id = @subjectId
            AND t.status = @activeStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tutorId", tutorId);
        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@activeStatus", (int)TutorStatus.Active);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

        if (result == null)
        {
            throw new TutorDoesNotTeachSubjectException(tutorId, subjectId);
        }

        return Convert.ToDecimal(result);
    }

    public async Task<bool> ValidateSlotBelongsToTutorAsync(Guid slotId, Guid tutorId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT 1 FROM schedule_slots WHERE id = @slotId AND tutor_id = @tutorId";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@slotId", slotId);
        command.Parameters.AddWithValue("@tutorId", tutorId);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<bool> ValidateTutorIsActiveAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT 1 FROM tutors WHERE id = @tutorId AND status = @activeStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tutorId", tutorId);
        command.Parameters.AddWithValue("@activeStatus", (int)TutorStatus.Active);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }
}