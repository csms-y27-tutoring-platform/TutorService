using Infrastructure.Persistence.Abstractions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Persistence.Connections;

internal class NpgsqlConnectionProvider : IConnectionProvider
{
    private readonly string _connectionString;

    public NpgsqlConnectionProvider(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("DefaultConnection is not configured");
    }

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        var npgsqlConnection = new NpgsqlConnectionWrapper(connection);
        await npgsqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return npgsqlConnection;
    }
}