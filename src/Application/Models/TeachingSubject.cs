namespace Application.Models;

public class TeachingSubject
{
    public Guid Id { get; private set; }

    public Guid TutorId { get; private set; }

    public Guid SubjectId { get; private set; }

    public decimal PricePerHour { get; private set; }

    public string? Description { get; private set; }

    public int ExperienceYears { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public TeachingSubject(Guid id, Guid tutorId, Guid subjectId, decimal pricePerHour, string? description, int experienceYears)
    {
        if (pricePerHour <= 0)
            throw new ArgumentException("Price per hour must be greater than 0", nameof(pricePerHour));

        if (experienceYears < 0)
            throw new ArgumentException("Experience years cannot be negative", nameof(experienceYears));

        Id = id;
        TutorId = tutorId;
        SubjectId = subjectId;
        PricePerHour = pricePerHour;
        Description = description;
        ExperienceYears = experienceYears;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(decimal pricePerHour, string? description, int experienceYears)
    {
        if (pricePerHour <= 0)
            throw new ArgumentException("Price per hour must be greater than 0", nameof(pricePerHour));

        if (experienceYears < 0)
            throw new ArgumentException("Experience years cannot be negative", nameof(experienceYears));

        PricePerHour = pricePerHour;
        Description = description;
        ExperienceYears = experienceYears;
        UpdatedAt = DateTime.UtcNow;
    }
}