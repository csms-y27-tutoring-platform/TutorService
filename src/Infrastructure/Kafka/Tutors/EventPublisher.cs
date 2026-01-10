using Application.Abstractions;
using Confluent.Kafka;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tutor.Events;

namespace Infrastructure.Kafka.Tutors;

public class EventPublisher : ITutorEventPublisher
{
    private readonly IProducer<Null, byte[]> _producer;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(
        IProducer<Null, byte[]> producer,
        ILogger<EventPublisher> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task PublishTutorCreatedAsync(Guid tutorId, string name, CancellationToken cancellationToken = default)
    {
        var message = new TutorCreatedEvent
        {
            TutorId = tutorId.ToString(),
            FullName = name,
            CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };

        await PublishEventAsync("tutor-created", message.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task PublishTutorUpdatedAsync(Guid tutorId, string name, CancellationToken cancellationToken = default)
    {
        var message = new TutorUpdatedEvent
        {
            TutorId = tutorId.ToString(),
            FullName = name,
            UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };

        await PublishEventAsync("tutor-updated", message.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task PublishScheduleUpdatedAsync(Guid tutorId, Guid slotId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        var message = new ScheduleUpdatedEvent
        {
            TutorId = tutorId.ToString(),
            SlotId = slotId.ToString(),
            StartTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(startTime),
            EndTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(endTime),
            UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };

        await PublishEventAsync("schedule-updated", message.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task PublishSlotReservedAsync(Guid slotId, Guid tutorId, CancellationToken cancellationToken = default)
    {
        var message = new SlotReservedEvent
        {
            SlotId = slotId.ToString(),
            TutorId = tutorId.ToString(),
            ReservedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };

        await PublishEventAsync("slot-reserved", message.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task PublishSlotReleasedAsync(Guid slotId, Guid tutorId, CancellationToken cancellationToken = default)
    {
        var message = new SlotReleasedEvent
        {
            SlotId = slotId.ToString(),
            TutorId = tutorId.ToString(),
            ReleasedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
        };

        await PublishEventAsync("slot-released", message.ToByteArray(), cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task PublishEventAsync(string topic, byte[] message, CancellationToken cancellationToken)
    {
        try
        {
            var kafkaMessage = new Message<Null, byte[]>
            {
                Value = message,
            };

            DeliveryResult<Null, byte[]> result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogDebug("Successfully published event to topic {Topic}, partition {Partition}, offset {Offset}", topic, result.Partition, result.Offset);
        }
        catch (ProduceException<Null, byte[]> ex)
        {
            _logger.LogError(ex, "Failed to publish event to topic {Topic}", topic);
            throw new InvalidOperationException($"Failed to publish event to topic {topic}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while publishing event to topic {Topic}", topic);
            throw;
        }
    }
}