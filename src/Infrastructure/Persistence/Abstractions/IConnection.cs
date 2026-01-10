using System.Data.Common;

namespace Infrastructure.Persistence.Abstractions;

public interface IConnection : IAsyncDisposable
{
    public ICommand CreateCommand(string sql);

    public Task OpenAsync(CancellationToken cancellationToken = default);

    public Task CloseAsync(CancellationToken cancellationToken = default);

    public DbTransaction? Transaction { get; }

    public Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}