namespace Application.Exceptions;

public class SubjectNotFoundException : Exception
{
    public SubjectNotFoundException(Guid subjectId)
        : base($"Subject with ID '{subjectId}' was not found.")
    {
        SubjectId = subjectId;
    }

    public Guid SubjectId { get; }
}