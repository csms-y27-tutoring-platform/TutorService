namespace Application.Exceptions;

public class TutorNotActiveException : Exception
{
    public TutorNotActiveException(Guid tutorId)
        : base($"Tutor with ID '{tutorId}' is not active.")
    {
        TutorId = tutorId;
    }

    public Guid TutorId { get; }
}