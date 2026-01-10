using Application.Abstractions;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence.Repositories;

public class TutorRepository : ITutorRepository
{
    private readonly PersistenceContext _context;

    public TutorRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<Application.Models.Tutor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tutors
            .Include(t => t.TeachingSubjects)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Application.Models.Tutor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tutors
            .Include(t => t.TeachingSubjects)
            .Where(t => t.Status != TutorStatus.Deleted)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Application.Models.Tutor> AddAsync(Application.Models.Tutor tutor, CancellationToken cancellationToken = default)
    {
        EntityEntry<Application.Models.Tutor> entry = await _context.Tutors.AddAsync(tutor, cancellationToken)
            .ConfigureAwait(false);
        return entry.Entity;
    }

    public async Task UpdateAsync(Application.Models.Tutor tutor, CancellationToken cancellationToken = default)
    {
        _context.Tutors.Update(tutor);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tutors
            .AnyAsync(t => t.Id == id && t.Status != TutorStatus.Deleted, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Application.Models.Tutor>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        return await _context.Tutors
            .Include(t => t.TeachingSubjects)
            .Where(t => t.TeachingSubjects.Any(ts => ts.SubjectId == subjectId)
                && t.Status == TutorStatus.Active)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Application.Models.Tutor?> GetByIdWithSubjectsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tutors
            .Include(t => t.TeachingSubjects)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }
}