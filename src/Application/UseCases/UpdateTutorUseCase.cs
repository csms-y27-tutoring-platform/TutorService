using Application.Abstractions;
using Application.Contracts.Tutors;
using Application.Exceptions;
using Application.Models;
using TutorService.Application.Models;

namespace Application.UseCases;

public class UpdateTutorUseCase
{
    private readonly ITutorRepository _tutorRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITutorEventPublisher _eventPublisher;

    public UpdateTutorUseCase(
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

    public async Task ExecuteAsync(Guid tutorId, UpdateTutorRequest request, CancellationToken cancellationToken)
    {
        Tutor? tutor = await _tutorRepository.GetByIdAsync(tutorId, cancellationToken)
            .ConfigureAwait(false);

        if (tutor is null)
        {
            throw new TutorNotFoundException(tutorId);
        }

        tutor.Update(
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Description,
            request.PreferredFormat,
            request.AverageLessonDurationMinutes);

        var currentSubjectIds = tutor.TeachingSubjects.Select(ts => ts.SubjectId).ToList();
        foreach (Guid subjectId in currentSubjectIds)
        {
            tutor.RemoveTeachingSubject(subjectId);
        }

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

        await _tutorRepository.UpdateAsync(tutor, cancellationToken)
            .ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        await _eventPublisher.PublishTutorUpdatedAsync(tutorId, tutor.FullName, cancellationToken)
            .ConfigureAwait(false);
    }
}