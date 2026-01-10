using Application.Abstractions;
using Application.Contracts.Schedule;
using Application.Exceptions;
using Application.Models;

namespace Application.UseCases;

public class CreateScheduleSlotUseCase
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IScheduleSlotRepository _slotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITutorEventPublisher _eventPublisher;

    public CreateScheduleSlotUseCase(
        ITutorRepository tutorRepository,
        IScheduleSlotRepository slotRepository,
        IUnitOfWork unitOfWork,
        ITutorEventPublisher eventPublisher)
    {
        _tutorRepository = tutorRepository;
        _slotRepository = slotRepository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> ExecuteAsync(CreateScheduleSlotRequest request, CancellationToken cancellationToken)
    {
        Tutor? tutor = await _tutorRepository.GetByIdAsync(request.TutorId, cancellationToken)
            .ConfigureAwait(false);

        if (tutor is null)
        {
            throw new TutorNotFoundException(request.TutorId);
        }

        if (!tutor.IsActive)
        {
            throw new TutorNotActiveException(request.TutorId);
        }

        var slotId = Guid.NewGuid();
        var slot = new ScheduleSlot(slotId, request.TutorId, request.StartTime, request.EndTime);

        await _slotRepository.AddAsync(slot, cancellationToken)
            .ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        await _eventPublisher.PublishScheduleUpdatedAsync(
                request.TutorId,
                slotId,
                request.StartTime,
                request.EndTime,
                cancellationToken)
            .ConfigureAwait(false);

        return slotId;
    }
}