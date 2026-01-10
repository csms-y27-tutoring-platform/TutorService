namespace Application.Exceptions;

public class TutorDoesNotTeachSubjectException : Exception
{
    public TutorDoesNotTeachSubjectException(Guid tutorId, Guid subjectId)
        : base($"Tutor with ID '{tutorId}' does not teach subject with ID '{subjectId}'.")
    {
        TutorId = tutorId;
        SubjectId = subjectId;
    }

    public Guid TutorId { get; }

    public Guid SubjectId { get; }
}
