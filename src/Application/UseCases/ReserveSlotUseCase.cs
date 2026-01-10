using Application.Abstractions;
using Application.Contracts.Schedule;
using Application.Exceptions;
using Application.Models;

namespace Application.UseCases;

public class ReserveSlotUseCase
{
    private readonly IScheduleSlotRepository _slotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITutorEventPublisher _eventPublisher;

    public ReserveSlotUseCase(
        IScheduleSlotRepository slotRepository,
        IUnitOfWork unitOfWork,
        ITutorEventPublisher eventPublisher)
    {
        _slotRepository = slotRepository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task ExecuteAsync(ReserveSlotRequest request, CancellationToken cancellationToken)
    {
        ScheduleSlot? slot = await _slotRepository.GetByIdAsync(request.SlotId, cancellationToken)
            .ConfigureAwait(false);

        if (slot is null)
        {
            throw new SlotNotFoundException(request.SlotId);
        }

        if (!slot.IsAvailable())
        {
            throw new SlotNotAvailableException(request.SlotId);
        }

        slot.Reserve(request.BookingId);

        await _slotRepository.UpdateAsync(slot, cancellationToken)
            .ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        await _eventPublisher.PublishSlotReservedAsync(
                request.SlotId,
                slot.TutorId,
                cancellationToken)
            .ConfigureAwait(false);
    }
}