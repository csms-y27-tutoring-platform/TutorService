using TutorService.Application.Models;

namespace Application.Abstractions;

public interface ISubjectRepository
{
    public Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<Subject>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<Subject> AddAsync(Subject subject, CancellationToken cancellationToken = default);
}