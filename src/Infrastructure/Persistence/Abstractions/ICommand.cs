using System.Data.Common;

namespace Infrastructure.Persistence.Abstractions;

public interface ICommand : IAsyncDisposable
{
    public ICommand AddParameter<T>(string name, T value);

    public ICommand AddParameter<T>(string name, IEnumerable<T> values);

    public Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default);

    public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default);

    public Task<bool> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default);
}