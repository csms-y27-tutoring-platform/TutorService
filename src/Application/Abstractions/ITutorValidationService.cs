namespace Application.Abstractions;

public interface ITutorValidationService
{
    public Task<bool> ValidateTutorExistsAsync(Guid tutorId, CancellationToken cancellationToken = default);

    public Task<bool> ValidateSubjectExistsAsync(Guid subjectId, CancellationToken cancellationToken = default);

    public Task<bool> ValidateTutorTeachesSubjectAsync(Guid tutorId, Guid subjectId, CancellationToken cancellationToken = default);

    public Task<bool> ValidateSlotAvailabilityAsync(Guid slotId, CancellationToken cancellationToken = default);

    public Task<decimal> GetLessonPriceAsync(Guid tutorId, Guid subjectId, CancellationToken cancellationToken = default);

    public Task<bool> ValidateSlotBelongsToTutorAsync(Guid slotId, Guid tutorId, CancellationToken cancellationToken = default);

    public Task<bool> ValidateTutorIsActiveAsync(Guid tutorId, CancellationToken cancellationToken = default);
}