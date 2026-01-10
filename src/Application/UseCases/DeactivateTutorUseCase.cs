using Application.Abstractions;
using Application.Exceptions;
using Application.Models;

namespace Application.UseCases;

public class DeactivateTutorUseCase
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITutorEventPublisher _eventPublisher;

    public DeactivateTutorUseCase(
        ITutorRepository tutorRepository,
        IUnitOfWork unitOfWork,
        ITutorEventPublisher eventPublisher)
    {
        _tutorRepository = tutorRepository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task ExecuteAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        Tutor? tutor = await _tutorRepository.GetByIdAsync(tutorId, cancellationToken)
            .ConfigureAwait(false);

        if (tutor == null)
        {
            throw new TutorNotFoundException(tutorId);
        }

        tutor.Deactivate();

        await _tutorRepository.UpdateAsync(tutor, cancellationToken)
            .ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        await _eventPublisher.PublishTutorUpdatedAsync(tutorId, tutor.FullName, cancellationToken)
            .ConfigureAwait(false);
    }
}