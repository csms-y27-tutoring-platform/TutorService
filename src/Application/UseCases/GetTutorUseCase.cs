using Application.Abstractions;
using Application.Contracts.Tutors;
using Application.Exceptions;
using Application.Models;
using TutorService.Application.Models;

namespace Application.UseCases;

public class GetTutorUseCase
{
    private readonly ITutorRepository _tutorRepository;
    private readonly ISubjectRepository _subjectRepository;

    public GetTutorUseCase(
        ITutorRepository tutorRepository,
        ISubjectRepository subjectRepository)
    {
        _tutorRepository = tutorRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<GetTutorResponse> ExecuteAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        Tutor? tutor = await _tutorRepository.GetByIdWithSubjectsAsync(tutorId, cancellationToken)
            .ConfigureAwait(false);

        if (tutor is null)
        {
            throw new TutorNotFoundException(tutorId);
        }

        var teachingSubjectResponses = new List<TeachingSubjectResponse>();

        foreach (TeachingSubject teachingSubject in tutor.TeachingSubjects)
        {
            Subject? subject = await _subjectRepository.GetByIdAsync(teachingSubject.SubjectId, cancellationToken)
                .ConfigureAwait(false);

            var teachingSubjectResponse = new TeachingSubjectResponse
            {
                Id = teachingSubject.Id,
                SubjectId = teachingSubject.SubjectId,
                SubjectName = subject?.Name ?? "Unknown Subject",
                PricePerHour = teachingSubject.PricePerHour,
                Description = teachingSubject.Description,
                ExperienceYears = teachingSubject.ExperienceYears,
            };

            teachingSubjectResponses.Add(teachingSubjectResponse);
        }

        return new GetTutorResponse
        {
            Id = tutor.Id,
            FirstName = tutor.FirstName,
            LastName = tutor.LastName,
            Email = tutor.Email,
            Phone = tutor.Phone,
            Description = tutor.Description,
            Status = tutor.Status,
            PreferredFormat = tutor.PreferredFormat,
            AverageLessonDurationMinutes = tutor.AverageLessonDurationMinutes,
            TeachingSubjects = teachingSubjectResponses,
            CreatedAt = tutor.CreatedAt,
            UpdatedAt = tutor.UpdatedAt,
        };
    }
}