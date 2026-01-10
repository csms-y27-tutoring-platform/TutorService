using Application.Models;

namespace Application.Abstractions;

public interface ITutorRepository
{
    public Task<Tutor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<Tutor>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task<Tutor> AddAsync(Tutor tutor, CancellationToken cancellationToken = default);

    public Task UpdateAsync(Tutor tutor, CancellationToken cancellationToken = default);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<Tutor>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default);

    public Task<Tutor?> GetByIdWithSubjectsAsync(Guid id, CancellationToken cancellationToken = default);
}