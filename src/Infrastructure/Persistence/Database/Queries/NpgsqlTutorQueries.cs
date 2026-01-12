using Application.Models;
using Npgsql;

namespace Infrastructure.Persistence.Database.Queries;

public class NpgsqlTutorQueries : ITutorQueries
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public NpgsqlTutorQueries(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Application.Models.Tutor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT t.id, t.first_name, t.last_name, t.email, t.phone, t.description, 
                   t.status, t.preferred_format, t.average_lesson_duration_minutes, 
                   t.created_at, t.updated_at
            FROM tutors t
            WHERE t.id = @id AND t.status != @deletedStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@deletedStatus", (int)TutorStatus.Deleted);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return MapTutor(reader);
        }

        return null;
    }

    public async Task<IReadOnlyCollection<Application.Models.Tutor>> GetAllAsync(CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT t.id, t.first_name, t.last_name, t.email, t.phone, t.description, 
                   t.status, t.preferred_format, t.average_lesson_duration_minutes, 
                   t.created_at, t.updated_at
            FROM tutors t
            WHERE t.status != @deletedStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@deletedStatus", (int)TutorStatus.Deleted);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var tutors = new List<Application.Models.Tutor>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            tutors.Add(MapTutor(reader));
        }

        return tutors;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT 1 FROM tutors WHERE id = @id AND status != @deletedStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@deletedStatus", (int)TutorStatus.Deleted);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<IReadOnlyCollection<Application.Models.Tutor>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            SELECT DISTINCT t.id, t.first_name, t.last_name, t.email, t.phone, t.description, 
                   t.status, t.preferred_format, t.average_lesson_duration_minutes, 
                   t.created_at, t.updated_at
            FROM tutors t
            INNER JOIN teaching_subjects ts ON t.id = ts.tutor_id
            WHERE ts.subject_id = @subjectId AND t.status = @activeStatus";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@subjectId", subjectId);
        command.Parameters.AddWithValue("@activeStatus", (int)TutorStatus.Active);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var tutors = new List<Application.Models.Tutor>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            tutors.Add(MapTutor(reader));
        }

        return tutors;
    }

    public async Task<Application.Models.Tutor?> GetByIdWithSubjectsAsync(Guid id, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string tutorSql = @"
            SELECT t.id, t.first_name, t.last_name, t.email, t.phone, t.description, 
                   t.status, t.preferred_format, t.average_lesson_duration_minutes, 
                   t.created_at, t.updated_at
            FROM tutors t
            WHERE t.id = @id AND t.status != @deletedStatus";

        const string subjectsSql = @"
            SELECT ts.id, ts.tutor_id, ts.subject_id, ts.price_per_hour, ts.description, 
                   ts.experience_years, ts.created_at, ts.updated_at, s.name as subject_name
            FROM teaching_subjects ts
            INNER JOIN subjects s ON ts.subject_id = s.id
            WHERE ts.tutor_id = @tutorId";

        using var tutorCommand = new NpgsqlCommand(tutorSql, connection);
        tutorCommand.Parameters.AddWithValue("@id", id);
        tutorCommand.Parameters.AddWithValue("@deletedStatus", (int)TutorStatus.Deleted);

        Application.Models.Tutor? tutor = null;
        using (NpgsqlDataReader reader = await tutorCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
        {
            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                tutor = MapTutor(reader);
            }
        }

        if (tutor == null)
        {
            return null;
        }

        using var subjectsCommand = new NpgsqlCommand(subjectsSql, connection);
        subjectsCommand.Parameters.AddWithValue("@tutorId", id);

        using NpgsqlDataReader subjectsReader = await subjectsCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var teachingSubjects = new List<TeachingSubject>();
        while (await subjectsReader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var teachingSubject = new TeachingSubject(
                subjectsReader.GetGuid(0), // id
                subjectsReader.GetGuid(1), // tutor_id
                subjectsReader.GetGuid(2), // subject_id
                subjectsReader.GetDecimal(3), // price_per_hour
                subjectsReader.IsDBNull(4) ? null : subjectsReader.GetString(4), // description
                subjectsReader.GetInt32(5)); // experience_years
            teachingSubjects.Add(teachingSubject);
        }

        return tutor;
    }

    private static Application.Models.Tutor MapTutor(NpgsqlDataReader reader)
    {
        var tutor = new Application.Models.Tutor(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.IsDBNull(4) ? null : reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            (LessonFormat)reader.GetInt32(7),
            reader.IsDBNull(8) ? null : reader.GetInt32(8));

        var status = (TutorStatus)reader.GetInt32(6);
        if (status == TutorStatus.Inactive) tutor.Deactivate();
        else if (status == TutorStatus.Suspended) tutor.Suspend();

        return tutor;
    }
}