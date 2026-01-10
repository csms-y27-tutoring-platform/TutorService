namespace Application.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}