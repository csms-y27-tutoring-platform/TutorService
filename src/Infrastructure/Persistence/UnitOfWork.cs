using Application.Abstractions;
using Infrastructure.Persistence.Database;
using Npgsql;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private NpgsqlTransaction? _transaction;
    private NpgsqlConnection? _connection;

    public UnitOfWork(IDatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return 0;
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return 1;
        }
        catch
        {
            await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync().ConfigureAwait(false);
            await DisposeConnectionAsync().ConfigureAwait(false);
        }
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        int result = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result > 0;
    }

    public async Task<NpgsqlTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection == null)
        {
            _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        }

        _transaction ??= await _connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        return _transaction;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeTransactionAsync().ConfigureAwait(false);
        await DisposeConnectionAsync().ConfigureAwait(false);
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }
    }

    private async Task DisposeConnectionAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
            _connection = null;
        }
    }
}