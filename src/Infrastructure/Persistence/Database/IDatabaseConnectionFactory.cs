using Npgsql;

namespace Infrastructure.Persistence.Database;

public interface IDatabaseConnectionFactory
{
    public Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);

    public NpgsqlConnection CreateConnection();
}