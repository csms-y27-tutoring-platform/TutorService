using Application.Abstractions;
using Application.Contracts.Schedule;
using Application.Exceptions;

namespace Application.UseCases;

public class ValidateSlotUseCase
{
    private readonly ITutorValidationService _validationService;

    public ValidateSlotUseCase(ITutorValidationService validationService)
    {
        _validationService = validationService;
    }

    public async Task<decimal> ExecuteAsync(SlotValidationRequest request, CancellationToken cancellationToken)
    {
        bool tutorExists = await _validationService.ValidateTutorExistsAsync(request.TutorId, cancellationToken)
            .ConfigureAwait(false);

        if (!tutorExists)
        {
            throw new TutorNotFoundException(request.TutorId);
        }

        bool tutorIsActive = await _validationService.ValidateTutorIsActiveAsync(request.TutorId, cancellationToken)
            .ConfigureAwait(false);

        if (!tutorIsActive)
        {
            throw new TutorNotActiveException(request.TutorId);
        }

        bool subjectExists = await _validationService.ValidateSubjectExistsAsync(request.SubjectId, cancellationToken)
            .ConfigureAwait(false);

        if (!subjectExists)
        {
            throw new SubjectNotFoundException(request.SubjectId);
        }

        bool tutorTeachesSubject = await _validationService.ValidateTutorTeachesSubjectAsync(
            request.TutorId,
            request.SubjectId,
            cancellationToken)
            .ConfigureAwait(false);

        if (!tutorTeachesSubject)
        {
            throw new TutorDoesNotTeachSubjectException(request.TutorId, request.SubjectId);
        }

        bool slotAvailable = await _validationService.ValidateSlotAvailabilityAsync(request.SlotId, cancellationToken)
            .ConfigureAwait(false);

        if (!slotAvailable)
        {
            throw new SlotNotAvailableException(request.SlotId);
        }

        bool slotBelongsToTutor = await _validationService.ValidateSlotBelongsToTutorAsync(
            request.SlotId,
            request.TutorId,
            cancellationToken)
            .ConfigureAwait(false);

        if (!slotBelongsToTutor)
        {
            throw new SlotNotFoundException(request.SlotId);
        }

        decimal price = await _validationService.GetLessonPriceAsync(request.TutorId, request.SubjectId, cancellationToken)
            .ConfigureAwait(false);

        return price;
    }
}