using FluentMigrator;

namespace Infrastructure.Persistence.Database.Migrations;

[Migration(003)]
public class SeedInitialData : Migration
{
    public override void Up()
    {
        Insert.IntoTable("subjects")
            .Row(new
            {
                id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                name = "Mathematics",
                description = "Mathematics including algebra, geometry, and calculus",
                created_at = DateTime.UtcNow,
            })
            .Row(new
            {
                id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                name = "Physics",
                description = "Fundamental principles of physics",
                created_at = DateTime.UtcNow,
            })
            .Row(new
            {
                id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa8"),
                name = "Chemistry",
                description = "Organic and inorganic chemistry",
                created_at = DateTime.UtcNow,
            })
            .Row(new
            {
                id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa9"),
                name = "Computer Science",
                description = "Programming, algorithms, and data structures",
                created_at = DateTime.UtcNow,
            })
            .Row(new
            {
                id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb0"),
                name = "English Language",
                description = "English grammar, literature, and writing",
                created_at = DateTime.UtcNow,
            });
    }

    public override void Down()
    {
        Delete.Table("subjects");
    }
}