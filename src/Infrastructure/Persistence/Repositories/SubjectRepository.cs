using Application.Abstractions;
using Infrastructure.Persistence.Database;
using Infrastructure.Persistence.Database.Queries;
using Npgsql;
using TutorService.Application.Models;

namespace Infrastructure.Persistence.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly ISubjectQueries _queries;

    public SubjectRepository(IDatabaseConnectionFactory connectionFactory, ISubjectQueries queries)
    {
        _connectionFactory = connectionFactory;
        _queries = queries;
    }

    public async Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _queries.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Subject>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _queries.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _queries.ExistsAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Subject> AddAsync(Subject subject, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);

        const string sql = @"
            INSERT INTO subjects (id, name, description, created_at)
            VALUES (@id, @name, @description, @createdAt)";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", subject.Id);
        command.Parameters.AddWithValue("@name", subject.Name);
        command.Parameters.AddWithValue("@description", subject.Description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@createdAt", subject.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        return subject;
    }
}