using Application.UseCases;
using Grpc.Core;
using Tutor.Service;
using static Tutor.Service.ScheduleService;

namespace Presentation.Grpc.Services;

public class ScheduleGrpcService : ScheduleServiceBase
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

    public override async Task<CreateScheduleSlotResponse> CreateScheduleSlot(
        global::Tutor.Service.CreateScheduleSlotRequest request,
        ServerCallContext context)
    {
        try
        {
            var createRequest = new Application.Contracts.Schedule.CreateScheduleSlotRequest
            {
                TutorId = Guid.Parse(request.TutorId),
                StartTime = request.StartTime.ToDateTime(),
                EndTime = request.EndTime.ToDateTime(),
            };

            Guid slotId = await _createScheduleSlotUseCase
                .ExecuteAsync(createRequest, context.CancellationToken)
                .ConfigureAwait(false);

            return new CreateScheduleSlotResponse
            {
                SlotId = slotId.ToString(),
            };
        }
        catch (Application.Exceptions.TutorNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Application.Exceptions.TutorNotActiveException ex)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {ex.Message}"));
        }
    }

    public override async Task<ReserveSlotResponse> ReserveSlot(
        global::Tutor.Service.ReserveSlotRequest request,
        ServerCallContext context)
    {
        try
        {
            var reserveRequest = new Application.Contracts.Schedule.ReserveSlotRequest
            {
                SlotId = Guid.Parse(request.SlotId),
                BookingId = Guid.Parse(request.BookingId),
            };

            await _reserveSlotUseCase
                .ExecuteAsync(reserveRequest, context.CancellationToken)
                .ConfigureAwait(false);

            return new ReserveSlotResponse
            {
                SlotId = request.SlotId,
                ReservedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
            };
        }
        catch (Application.Exceptions.SlotNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Application.Exceptions.SlotNotAvailableException ex)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {ex.Message}"));
        }
    }

    public override async Task<ReleaseSlotResponse> ReleaseSlot(
        ReleaseSlotRequest request,
        ServerCallContext context)
    {
        try
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
        catch (Application.Exceptions.SlotNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {ex.Message}"));
        }
    }
}