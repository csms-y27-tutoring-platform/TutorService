using Npgsql;
using TutorService.Application.Models;

namespace Infrastructure.Persistence.Database.Queries;

public class NpgsqlSubjectQueries : ISubjectQueries
{
    private readonly IDatabaseConnectionFactory _connectionFactory;

    public NpgsqlSubjectQueries(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT id, name, description, created_at, updated_at FROM subjects WHERE id = @id";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return MapSubject(reader);
        }

        return null;
    }

    public async Task<IReadOnlyCollection<Subject>> GetAllAsync(CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT id, name, description, created_at, updated_at FROM subjects ORDER BY name";

        using var command = new NpgsqlCommand(sql, connection);

        using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        var subjects = new List<Subject>();
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            subjects.Add(MapSubject(reader));
        }

        return subjects;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = "SELECT 1 FROM subjects WHERE id = @id";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    private static Subject MapSubject(NpgsqlDataReader reader)
    {
        return new Subject(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2));
    }
}