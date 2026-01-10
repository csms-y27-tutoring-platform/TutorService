using Google.Protobuf.WellKnownTypes;

namespace Application.Contracts.Tutors;

public class CreateTutorResponse
{
    public required Guid TutorId { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required Models.TutorStatus Status { get; init; }

    public required Timestamp CreatedAt { get; init; }
}