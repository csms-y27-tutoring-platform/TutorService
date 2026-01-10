using Application.Abstractions;
using Application.Exceptions;
using Application.Models;

namespace Application.UseCases;

public class GetTutorUseCase
{
    private readonly ITutorRepository _tutorRepository;

    public GetTutorUseCase(ITutorRepository tutorRepository)
    {
        _tutorRepository = tutorRepository;
    }

    public async Task<Tutor> ExecuteAsync(Guid tutorId, CancellationToken cancellationToken)
    {
        Tutor? tutor = await _tutorRepository.GetByIdWithSubjectsAsync(tutorId, cancellationToken)
            .ConfigureAwait(false);

        if (tutor == null)
        {
            throw new TutorNotFoundException(tutorId);
        }

        return tutor;
    }
}