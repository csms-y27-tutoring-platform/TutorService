using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TutorService.Application.Models;

namespace Infrastructure.Persistence.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly PersistenceContext _context;

    public SubjectRepository(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Subject>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Subjects
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Subjects
            .AnyAsync(s => s.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Subject> AddAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        EntityEntry<Subject> entry = await _context.Subjects.AddAsync(subject, cancellationToken)
            .ConfigureAwait(false);
        return entry.Entity;
    }
}