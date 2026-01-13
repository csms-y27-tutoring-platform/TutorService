using Application.Abstractions;
using Application.Models;
using Infrastructure.Persistence.Database;
using Infrastructure.Persistence.Database.Queries;
using Npgsql;

namespace Infrastructure.Persistence.Repositories;

public class TutorRepository : ITutorRepository
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly ITutorQueries _queries;

    public TutorRepository(IDatabaseConnectionFactory connectionFactory, ITutorQueries queries)
    {
        _connectionFactory = connectionFactory;
        _queries = queries;
    }

    public async Task<Application.Models.Tutor?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _queries.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Application.Models.Tutor>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _queries.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<Application.Models.Tutor> AddAsync(Application.Models.Tutor tutor, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            const string insertTutorSql = @"
                INSERT INTO tutors (id, first_name, last_name, email, phone, description, status, preferred_format, average_lesson_duration_minutes, created_at)
                VALUES (@id, @firstName, @lastName, @email, @phone, @description, @status, @preferredFormat, @averageLessonDurationMinutes, @createdAt)";

            using var tutorCommand = new NpgsqlCommand(insertTutorSql, connection, transaction);
            tutorCommand.Parameters.AddWithValue("@id", tutor.Id);
            tutorCommand.Parameters.AddWithValue("@firstName", tutor.FirstName);
            tutorCommand.Parameters.AddWithValue("@lastName", tutor.LastName);
            tutorCommand.Parameters.AddWithValue("@email", tutor.Email);
            tutorCommand.Parameters.AddWithValue("@phone", tutor.Phone ?? (object)DBNull.Value);
            tutorCommand.Parameters.AddWithValue("@description", tutor.Description ?? (object)DBNull.Value);
            tutorCommand.Parameters.AddWithValue("@status", (int)tutor.Status);
            tutorCommand.Parameters.AddWithValue("@preferredFormat", (int)tutor.PreferredFormat);
            tutorCommand.Parameters.AddWithValue("@averageLessonDurationMinutes", tutor.AverageLessonDurationMinutes ?? (object)DBNull.Value);
            tutorCommand.Parameters.AddWithValue("@createdAt", tutor.CreatedAt);

            await tutorCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            foreach (TeachingSubject teachingSubject in tutor.TeachingSubjects)
            {
                const string insertTeachingSubjectSql = @"
                    INSERT INTO teaching_subjects (id, tutor_id, subject_id, price_per_hour, description, experience_years, created_at)
                    VALUES (@id, @tutorId, @subjectId, @pricePerHour, @description, @experienceYears, @createdAt)";

                using var subjectCommand = new NpgsqlCommand(insertTeachingSubjectSql, connection, transaction);
                subjectCommand.Parameters.AddWithValue("@id", teachingSubject.Id);
                subjectCommand.Parameters.AddWithValue("@tutorId", teachingSubject.TutorId);
                subjectCommand.Parameters.AddWithValue("@subjectId", teachingSubject.SubjectId);
                subjectCommand.Parameters.AddWithValue("@pricePerHour", teachingSubject.PricePerHour);
                subjectCommand.Parameters.AddWithValue("@description", teachingSubject.Description ?? (object)DBNull.Value);
                subjectCommand.Parameters.AddWithValue("@experienceYears", teachingSubject.ExperienceYears);
                subjectCommand.Parameters.AddWithValue("@createdAt", teachingSubject.CreatedAt);

                await subjectCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return tutor;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task UpdateAsync(Application.Models.Tutor tutor, CancellationToken cancellationToken)
    {
        using NpgsqlConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            const string updateTutorSql = @"
                UPDATE tutors 
                SET first_name = @firstName,
                    last_name = @lastName,
                    phone = @phone,
                    description = @description,
                    preferred_format = @preferredFormat,
                    average_lesson_duration_minutes = @averageLessonDurationMinutes,
                    updated_at = @updatedAt
                WHERE id = @id";

            using var tutorCommand = new NpgsqlCommand(updateTutorSql, connection, transaction);
            tutorCommand.Parameters.AddWithValue("@id", tutor.Id);
            tutorCommand.Parameters.AddWithValue("@firstName", tutor.FirstName);
            tutorCommand.Parameters.AddWithValue("@lastName", tutor.LastName);
            tutorCommand.Parameters.AddWithValue("@phone", tutor.Phone ?? (object)DBNull.Value);
            tutorCommand.Parameters.AddWithValue("@description", tutor.Description ?? (object)DBNull.Value);
            tutorCommand.Parameters.AddWithValue("@preferredFormat", (int)tutor.PreferredFormat);
            tutorCommand.Parameters.AddWithValue("@averageLessonDurationMinutes", tutor.AverageLessonDurationMinutes ?? (object)DBNull.Value);
            tutorCommand.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);

            await tutorCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            const string deleteTeachingSubjectsSql = "DELETE FROM teaching_subjects WHERE tutor_id = @tutorId";
            using var deleteCommand = new NpgsqlCommand(deleteTeachingSubjectsSql, connection, transaction);
            deleteCommand.Parameters.AddWithValue("@tutorId", tutor.Id);
            await deleteCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            foreach (TeachingSubject teachingSubject in tutor.TeachingSubjects)
            {
                const string insertTeachingSubjectSql = @"
                    INSERT INTO teaching_subjects (id, tutor_id, subject_id, price_per_hour, description, experience_years, created_at)
                    VALUES (@id, @tutorId, @subjectId, @pricePerHour, @description, @experienceYears, @createdAt)";

                using var subjectCommand = new NpgsqlCommand(insertTeachingSubjectSql, connection, transaction);
                subjectCommand.Parameters.AddWithValue("@id", teachingSubject.Id);
                subjectCommand.Parameters.AddWithValue("@tutorId", teachingSubject.TutorId);
                subjectCommand.Parameters.AddWithValue("@subjectId", teachingSubject.SubjectId);
                subjectCommand.Parameters.AddWithValue("@pricePerHour", teachingSubject.PricePerHour);
                subjectCommand.Parameters.AddWithValue("@description", teachingSubject.Description ?? (object)DBNull.Value);
                subjectCommand.Parameters.AddWithValue("@experienceYears", teachingSubject.ExperienceYears);
                subjectCommand.Parameters.AddWithValue("@createdAt", teachingSubject.CreatedAt);

                await subjectCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _queries.ExistsAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Application.Models.Tutor>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken)
    {
        return await _queries.GetBySubjectIdAsync(subjectId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Application.Models.Tutor?> GetByIdWithSubjectsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _queries.GetByIdWithSubjectsAsync(id, cancellationToken).ConfigureAwait(false);
    }
}