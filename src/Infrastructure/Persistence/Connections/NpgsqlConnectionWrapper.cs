using Infrastructure.Persistence.Abstractions;
using Npgsql;
using System.Data.Common;

namespace Infrastructure.Persistence.Connections;

internal class NpgsqlConnectionWrapper : IConnection
{
    private readonly NpgsqlConnection _connection;
    private NpgsqlTransaction? _transaction;

    public NpgsqlConnectionWrapper(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public DbTransaction? Transaction => _transaction;

    public ICommand CreateCommand(string sql)
    {
        NpgsqlCommand command = _connection.CreateCommand();
        command.CommandText = sql;
        return new NpgsqlCommandWrapper(command);
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        await _connection.CloseAsync().ConfigureAwait(false);
    }

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        return _transaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync().ConfigureAwait(false);
        }

        await _connection.DisposeAsync().ConfigureAwait(false);
    }
}