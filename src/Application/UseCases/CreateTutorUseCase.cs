using Application.Abstractions;
using Application.Contracts.Tutors;
using Application.Exceptions;
using Application.Models;

namespace Application.UseCases;

public class CreateTutorUseCase
{
    private readonly ITutorRepository _tutorRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITutorEventPublisher _eventPublisher;

    public CreateTutorUseCase(
        ITutorRepository tutorRepository,
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork,
        ITutorEventPublisher eventPublisher)
    {
        _tutorRepository = tutorRepository;
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
    }

    public async Task<CreateTutorResponse> ExecuteAsync(CreateTutorRequest request, CancellationToken cancellationToken)
    {
        var tutorId = Guid.NewGuid();
        var tutor = new Tutor(
            tutorId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Description,
            request.PreferredFormat,
            request.AverageLessonDurationMinutes,
            DateTime.UtcNow);

        foreach (TeachingSubjectRequest subjectRequest in request.TeachingSubjects)
        {
            Subject? subject = await _subjectRepository.GetByIdAsync(subjectRequest.SubjectId, cancellationToken)
                .ConfigureAwait(false);

            if (subject is null)
            {
                throw new SubjectNotFoundException(subjectRequest.SubjectId);
            }

            var teachingSubjectId = Guid.NewGuid();
            var teachingSubject = new TeachingSubject(
                teachingSubjectId,
                tutorId,
                subjectRequest.SubjectId,
                subjectRequest.PricePerHour,
                subjectRequest.Description,
                subjectRequest.ExperienceYears);

            tutor.AddTeachingSubject(teachingSubject);
        }

        await _tutorRepository.AddAsync(tutor, cancellationToken)
            .ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        await _eventPublisher.PublishTutorCreatedAsync(
            tutorId,
            tutor.FullName,
            cancellationToken)
            .ConfigureAwait(false);

        return new CreateTutorResponse
        {
            TutorId = tutorId,
            FullName = tutor.FullName,
            Email = tutor.Email,
            Status = tutor.Status,
            CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(tutor.CreatedAt),
        };
    }
}