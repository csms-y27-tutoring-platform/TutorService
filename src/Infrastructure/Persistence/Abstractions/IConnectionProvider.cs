namespace Infrastructure.Persistence.Abstractions;

public interface IConnectionProvider
{
    public Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
}