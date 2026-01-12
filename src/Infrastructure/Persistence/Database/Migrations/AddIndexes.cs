using FluentMigrator;

namespace Infrastructure.Persistence.Database.Migrations;

[Migration(002)]
public class AddIndexes : Migration
{
    public override void Up()
    {
        Create.Index("ix_tutors_status")
            .OnTable("tutors")
            .OnColumn("status").Ascending();

        Create.Index("ix_tutors_email")
            .OnTable("tutors")
            .OnColumn("email").Ascending()
            .WithOptions().Unique();

        Create.Index("ix_subjects_name")
            .OnTable("subjects")
            .OnColumn("name").Ascending()
            .WithOptions().Unique();

        Create.Index("ix_schedule_slots_tutor_id")
            .OnTable("schedule_slots")
            .OnColumn("tutor_id").Ascending();

        Create.Index("ix_schedule_slots_booking_id")
            .OnTable("schedule_slots")
            .OnColumn("booking_id").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Index("ix_schedule_slots_booking_id").OnTable("schedule_slots");
        Delete.Index("ix_schedule_slots_tutor_id").OnTable("schedule_slots");
        Delete.Index("ix_subjects_name").OnTable("subjects");
        Delete.Index("ix_tutors_email").OnTable("tutors");
        Delete.Index("ix_tutors_status").OnTable("tutors");
    }
}