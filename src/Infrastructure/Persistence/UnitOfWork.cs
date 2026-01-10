using Application.Abstractions;
using Infrastructure.Persistence.Abstractions;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IConnectionProvider _connectionProvider;
    private IConnection? _connection;
    private bool _transactionStarted;

    public UnitOfWork(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_connection == null)
        {
            return 0;
        }

        if (_transactionStarted)
        {
            try
            {
                await _connection.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                _transactionStarted = false;
            }
            catch
            {
                await _connection.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                _transactionStarted = false;
                throw;
            }
        }

        return 1;
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        int result = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result > 0;
    }

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection == null)
        {
            _connection = await _connectionProvider.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
            await _connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            _transactionStarted = true;
        }

        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
            _connection = null;
        }
    }
}