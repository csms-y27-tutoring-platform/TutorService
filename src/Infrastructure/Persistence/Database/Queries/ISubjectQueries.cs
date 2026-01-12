using TutorService.Application.Models;

namespace Infrastructure.Persistence.Database.Queries;

public interface ISubjectQueries
{
    public Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<Subject>> GetAllAsync(CancellationToken cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}