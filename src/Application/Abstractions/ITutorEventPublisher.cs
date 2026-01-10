namespace Application.Abstractions;

public interface ITutorEventPublisher
{
    public Task PublishTutorCreatedAsync(Guid tutorId, string name, CancellationToken cancellationToken = default);

    public Task PublishTutorUpdatedAsync(Guid tutorId, string name, CancellationToken cancellationToken = default);

    public Task PublishScheduleUpdatedAsync(Guid tutorId, Guid slotId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

    public Task PublishSlotReservedAsync(Guid slotId, Guid tutorId, CancellationToken cancellationToken = default);

    public Task PublishSlotReleasedAsync(Guid slotId, Guid tutorId, CancellationToken cancellationToken = default);
}