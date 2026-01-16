using Application.UseCases;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tutor.Service;
using CreateTutorRequest = Application.Contracts.Tutors.CreateTutorRequest;
using CreateTutorResponse = Application.Contracts.Tutors.CreateTutorResponse;
using GetTutorResponse = Tutor.Service.GetTutorResponse;
using TeachingSubjectRequest = Application.Contracts.Tutors.TeachingSubjectRequest;
using TeachingSubjectResponse = Tutor.Service.TeachingSubjectResponse;
using UpdateTutorRequest = Application.Contracts.Tutors.UpdateTutorRequest;

namespace Presentation.Grpc.Services;

public class TutorGrpcService : Tutor.Service.TutorService.TutorServiceBase
{
    private readonly CreateTutorUseCase _createTutorUseCase;
    private readonly UpdateTutorUseCase _updateTutorUseCase;
    private readonly GetTutorUseCase _getTutorUseCase;
    private readonly DeactivateTutorUseCase _deactivateTutorUseCase;

    public TutorGrpcService(
        CreateTutorUseCase createTutorUseCase,
        UpdateTutorUseCase updateTutorUseCase,
        GetTutorUseCase getTutorUseCase,
        DeactivateTutorUseCase deactivateTutorUseCase)
    {
        _createTutorUseCase = createTutorUseCase;
        _updateTutorUseCase = updateTutorUseCase;
        _getTutorUseCase = getTutorUseCase;
        _deactivateTutorUseCase = deactivateTutorUseCase;
    }

    public override async Task<Tutor.Service.CreateTutorResponse> CreateTutor(
        Tutor.Service.CreateTutorRequest request,
        ServerCallContext context)
    {
        var teachingSubjects = request.TeachingSubjects
            .Select(ts => new TeachingSubjectRequest
            {
                SubjectId = new Guid(ts.SubjectId),
                PricePerHour = (decimal)ts.PricePerHour,
                Description = ts.Description,
                ExperienceYears = ts.ExperienceYears,
            })
            .ToList();

        var createTutorRequest = new CreateTutorRequest
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Description = request.Description,
            PreferredFormat = (Application.Models.LessonFormat)request.PreferredFormat,
            AverageLessonDurationMinutes = request.AverageLessonDurationMinutes,
            TeachingSubjects = teachingSubjects,
        };

        CreateTutorResponse result = await _createTutorUseCase
            .ExecuteAsync(createTutorRequest, context.CancellationToken)
            .ConfigureAwait(false);

        return new Tutor.Service.CreateTutorResponse
        {
            TutorId = result.TutorId.ToString(),
            FullName = result.FullName,
            Email = result.Email,
            Status = (Tutor.Service.TutorStatus)result.Status,
            CreatedAt = result.CreatedAt,
        };
    }

    public override async Task<Tutor.Service.UpdateTutorResponse> UpdateTutor(
        Tutor.Service.UpdateTutorRequest request,
        ServerCallContext context)
    {
        var teachingSubjects = request.TeachingSubjects
            .Select(ts => new TeachingSubjectRequest
            {
                SubjectId = new Guid(ts.SubjectId),
                PricePerHour = (decimal)ts.PricePerHour,
                Description = ts.Description,
                ExperienceYears = ts.ExperienceYears,
            })
            .ToList();

        var updateTutorRequest = new UpdateTutorRequest
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Description = request.Description,
            PreferredFormat = (Application.Models.LessonFormat)request.PreferredFormat,
            AverageLessonDurationMinutes = request.AverageLessonDurationMinutes,
            TeachingSubjects = teachingSubjects,
            TutorId = default,
        };

        await _updateTutorUseCase
            .ExecuteAsync(Guid.Parse(request.TutorId), updateTutorRequest, context.CancellationToken)
            .ConfigureAwait(false);

        return new Tutor.Service.UpdateTutorResponse
        {
            TutorId = request.TutorId,
            UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
        };
    }

    public override async Task<GetTutorResponse> GetTutor(
        GetTutorRequest request,
        ServerCallContext context)
    {
        try
        {
            Application.Contracts.Tutors.GetTutorResponse result = await _getTutorUseCase
                .ExecuteAsync(Guid.Parse(request.TutorId), context.CancellationToken)
                .ConfigureAwait(false);

            var teachingSubjectResponses = result.TeachingSubjects
                .Select<Application.Contracts.Tutors.TeachingSubjectResponse, TeachingSubjectResponse>(ts => new TeachingSubjectResponse
                {
                    SubjectId = ts.SubjectId.ToString(),
                    SubjectName = ts.SubjectName,
                    PricePerHour = (double)ts.PricePerHour,
                    Description = ts.Description ?? string.Empty,
                    ExperienceYears = ts.ExperienceYears,
                })
                .ToList();

            var response = new GetTutorResponse
            {
                TutorId = result.Id.ToString(),
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                Phone = result.Phone ?? string.Empty,
                Description = result.Description ?? string.Empty,
                Status = (Tutor.Service.TutorStatus)result.Status,
                PreferredFormat = (Tutor.Service.LessonFormat)result.PreferredFormat,
                CreatedAt = result.CreatedAt,
            };

            if (result.AverageLessonDurationMinutes.HasValue)
            {
                response.AverageLessonDurationMinutes = result.AverageLessonDurationMinutes.Value;
            }

            response.TeachingSubjects.AddRange(teachingSubjectResponses);

            return response;
        }
        catch (Application.Exceptions.TutorNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<Tutor.Service.DeactivateTutorResponse> DeactivateTutor(
        Tutor.Service.DeactivateTutorRequest request,
        ServerCallContext context)
    {
        try
        {
            await _deactivateTutorUseCase
                .ExecuteAsync(Guid.Parse(request.TutorId), context.CancellationToken)
                .ConfigureAwait(false);

            return new Tutor.Service.DeactivateTutorResponse
            {
                TutorId = request.TutorId,
                DeactivatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            };
        }
        catch (Application.Exceptions.TutorNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}