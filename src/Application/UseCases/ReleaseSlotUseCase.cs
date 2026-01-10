using Application.Abstractions;
using Application.Exceptions;
using Application.Models;

namespace Application.UseCases;

public class ReleaseSlotUseCase
{
    private readonly IScheduleSlotRepository _slotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITutorEventPublisher _eventPublisher;

    public ReleaseSlotUseCase(
        IScheduleSlotRepository slotRepository,
        IUnitOfWork unitOfWork,
        ITutorEventPublisher eventPublisher)
    {
        _slotRepository = slotRepository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task ExecuteAsync(Guid slotId, CancellationToken cancellationToken)
    {
        ScheduleSlot? slot = await _slotRepository.GetByIdAsync(slotId, cancellationToken)
            .ConfigureAwait(false);

        if (slot is null)
        {
            throw new SlotNotFoundException(slotId);
        }

        if (slot.IsAvailable())
        {
            throw new InvalidOperationException("Slot is already available");
        }

        slot.Release();

        await _slotRepository.UpdateAsync(slot, cancellationToken)
            .ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        await _eventPublisher.PublishSlotReleasedAsync(slotId, slot.TutorId, cancellationToken)
            .ConfigureAwait(false);
    }
}