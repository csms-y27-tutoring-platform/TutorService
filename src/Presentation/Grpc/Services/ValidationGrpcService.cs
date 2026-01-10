using Application.Contracts.Schedule;
using Application.UseCases;
using Grpc.Core;
using Tutor.Service;

namespace Presentation.Grpc.Services;

public class ValidationGrpcService : ValidationService.ValidationServiceBase
{
    private readonly ValidateSlotUseCase _validateSlotUseCase;

    public ValidationGrpcService(ValidateSlotUseCase validateSlotUseCase)
    {
        _validateSlotUseCase = validateSlotUseCase;
    }

    public override async Task<ValidateSlotResponse> ValidateSlot(
        ValidateSlotRequest request,
        ServerCallContext context)
    {
        try
        {
            var validationRequest = new SlotValidationRequest
            {
                SlotId = Guid.Parse(request.SlotId),
                TutorId = Guid.Parse(request.TutorId),
                SubjectId = Guid.Parse(request.SubjectId),
            };

            decimal price = await _validateSlotUseCase
                .ExecuteAsync(validationRequest, context.CancellationToken)
                .ConfigureAwait(false);

            return new ValidateSlotResponse
            {
                IsValid = true,
                PricePerHour = (double)price,
            };
        }
        catch (Application.Exceptions.TutorNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Application.Exceptions.SubjectNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Application.Exceptions.TutorNotActiveException ex)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        }
        catch (Application.Exceptions.TutorDoesNotTeachSubjectException ex)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
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
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}