using Application.Abstractions;
using Application.Exceptions;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class TutorValidationService : ITutorValidationService
{
    private readonly PersistenceContext _context;

    public TutorValidationService(PersistenceContext context)
    {
        _context = context;
    }

    public async Task<bool> ValidateTutorExistsAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        return await _context.Tutors
            .AnyAsync(t => t.Id == tutorId && t.Status != TutorStatus.Deleted, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> ValidateSubjectExistsAsync(Guid subjectId, CancellationToken cancellationToken)
    {
        return await _context.Subjects
            .AnyAsync(s => s.Id == subjectId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> ValidateTutorTeachesSubjectAsync(Guid tutorId, Guid subjectId, CancellationToken cancellationToken)
    {
        return await _context.TeachingSubjects
            .AnyAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> ValidateSlotAvailabilityAsync(Guid slotId, CancellationToken cancellationToken)
    {
        ScheduleSlot? slot = await _context.ScheduleSlots
            .FirstOrDefaultAsync(s => s.Id == slotId, cancellationToken)
            .ConfigureAwait(false);

        return slot?.Status == SlotStatus.Available;
    }

    public async Task<decimal> GetLessonPriceAsync(Guid tutorId, Guid subjectId, CancellationToken cancellationToken)
    {
        TeachingSubject? teachingSubject = await _context.TeachingSubjects
            .FirstOrDefaultAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId, cancellationToken)
            .ConfigureAwait(false);

        if (teachingSubject == null)
        {
            throw new TutorDoesNotTeachSubjectException(tutorId, subjectId);
        }

        return teachingSubject.PricePerHour;
    }

    public async Task<bool> ValidateSlotBelongsToTutorAsync(Guid slotId, Guid tutorId, CancellationToken cancellationToken)
    {
        return await _context.ScheduleSlots
            .AnyAsync(s => s.Id == slotId && s.TutorId == tutorId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> ValidateTutorIsActiveAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        Application.Models.Tutor? tutor = await _context.Tutors
            .FirstOrDefaultAsync(t => t.Id == tutorId, cancellationToken)
            .ConfigureAwait(false);

        return tutor?.Status == TutorStatus.Active;
    }
}