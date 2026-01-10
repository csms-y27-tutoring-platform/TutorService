namespace Application.Exceptions;

public class TutorNotFoundException : Exception
{
    public TutorNotFoundException(Guid tutorId)
        : base($"Tutor with ID '{tutorId}' was not found.")
    {
        TutorId = tutorId;
    }

    public Guid TutorId { get; }
}