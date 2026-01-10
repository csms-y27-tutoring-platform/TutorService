using Application.Models;
using Application.UseCases;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using CreateTutorRequest = Application.Contracts.Tutors.CreateTutorRequest;
using CreateTutorResponse = Application.Contracts.Tutors.CreateTutorResponse;
using TeachingSubjectRequest = Application.Contracts.Tutors.TeachingSubjectRequest;
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
            UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };
    }

    public override async Task<Tutor.Service.GetTutorResponse> GetTutor(
        Tutor.Service.GetTutorRequest request,
        ServerCallContext context)
    {
        try
        {
            Application.Models.Tutor tutor = await _getTutorUseCase
                .ExecuteAsync(Guid.Parse(request.TutorId), context.CancellationToken)
                .ConfigureAwait(false);

            var teachingSubjectResponses = new List<Tutor.Service.TeachingSubjectResponse>();
            foreach (TeachingSubject teachingSubject in tutor.TeachingSubjects)
            {
                // TODO: Здесь нужно получить имя предмета из SubjectRepository
                string subjectName = "Subject";

                teachingSubjectResponses.Add(new Tutor.Service.TeachingSubjectResponse
                {
                    TeachingSubjectId = teachingSubject.Id.ToString(),
                    SubjectId = teachingSubject.SubjectId.ToString(),
                    SubjectName = subjectName,
                    PricePerHour = (double)teachingSubject.PricePerHour,
                    Description = teachingSubject.Description ?? string.Empty,
                    ExperienceYears = teachingSubject.ExperienceYears,
                });
            }

            return new Tutor.Service.GetTutorResponse
            {
                TutorId = tutor.Id.ToString(),
                FirstName = tutor.FirstName,
                LastName = tutor.LastName,
                Email = tutor.Email,
                Phone = tutor.Phone ?? string.Empty,
                Description = tutor.Description ?? string.Empty,
                Status = (Tutor.Service.TutorStatus)tutor.Status,
                PreferredFormat = (Tutor.Service.LessonFormat)tutor.PreferredFormat,
                AverageLessonDurationMinutes = tutor.AverageLessonDurationMinutes ?? 0,
                TeachingSubjects = { teachingSubjectResponses },
                CreatedAt = tutor.CreatedAt,
                UpdatedAt = tutor.UpdatedAt.HasValue
                    ? Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(tutor.UpdatedAt.Value)
                    : null,
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