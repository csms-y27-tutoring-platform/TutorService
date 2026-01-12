namespace Infrastructure.Persistence.Database.Queries;

public interface ITutorQueries
{
    public Task<Application.Models.Tutor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<Application.Models.Tutor>> GetAllAsync(CancellationToken cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<Application.Models.Tutor>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken);

    public Task<Application.Models.Tutor?> GetByIdWithSubjectsAsync(Guid id, CancellationToken cancellationToken);
}