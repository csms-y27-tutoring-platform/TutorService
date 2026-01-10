using Application.UseCases;
using Grpc.Core;
using Tutor.Service;
using CreateScheduleSlotRequest = Application.Contracts.Schedule.CreateScheduleSlotRequest;
using ReserveSlotRequest = Application.Contracts.Schedule.ReserveSlotRequest;

namespace Presentation.Grpc.Services;

public class ScheduleGrpcService : ScheduleService.ScheduleServiceBase
{
    private readonly CreateScheduleSlotUseCase _createScheduleSlotUseCase;
    private readonly ReserveSlotUseCase _reserveSlotUseCase;
    private readonly ReleaseSlotUseCase _releaseSlotUseCase;

    public ScheduleGrpcService(
        CreateScheduleSlotUseCase createScheduleSlotUseCase,
        ReserveSlotUseCase reserveSlotUseCase,
        ReleaseSlotUseCase releaseSlotUseCase)
    {
        _createScheduleSlotUseCase = createScheduleSlotUseCase;
        _reserveSlotUseCase = reserveSlotUseCase;
        _releaseSlotUseCase = releaseSlotUseCase;
    }

    public async Task<CreateScheduleSlotResponse> CreateScheduleSlot(
        CreateScheduleSlotRequest request,
        ServerCallContext context)
    {
        var createRequest = new CreateScheduleSlotRequest
        {
            TutorId = request.TutorId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
        };

        Guid slotId = await _createScheduleSlotUseCase
            .ExecuteAsync(createRequest, context.CancellationToken)
            .ConfigureAwait(false);

        return new CreateScheduleSlotResponse
        {
            SlotId = slotId.ToString(),
        };
    }

    public async Task<ReserveSlotResponse> ReserveSlot(
        ReserveSlotRequest request,
        ServerCallContext context)
    {
        var reserveRequest = new ReserveSlotRequest
        {
            SlotId = request.SlotId,
            BookingId = request.BookingId,
        };

        await _reserveSlotUseCase
            .ExecuteAsync(reserveRequest, context.CancellationToken)
            .ConfigureAwait(false);

        return new ReserveSlotResponse
        {
            SlotId = request.SlotId.ToString(),
            ReservedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };
    }

    public override async Task<ReleaseSlotResponse> ReleaseSlot(
        ReleaseSlotRequest request,
        ServerCallContext context)
    {
        await _releaseSlotUseCase
            .ExecuteAsync(Guid.Parse(request.SlotId), context.CancellationToken)
            .ConfigureAwait(false);

        return new ReleaseSlotResponse
        {
            SlotId = request.SlotId,
            ReleasedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };
    }
}