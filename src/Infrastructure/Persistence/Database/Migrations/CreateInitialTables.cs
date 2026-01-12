using FluentMigrator;

namespace Infrastructure.Persistence.Database.Migrations;

[Migration(001)]
public class CreateInitialTables : Migration
{
    public override void Up()
    {
        Create.Table("subjects")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("description").AsString(500).Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("updated_at").AsDateTime().Nullable();

        Create.Table("tutors")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("first_name").AsString(100).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("email").AsString(200).NotNullable()
            .WithColumn("phone").AsString(20).Nullable()
            .WithColumn("description").AsString(1000).Nullable()
            .WithColumn("status").AsInt32().NotNullable()
            .WithColumn("preferred_format").AsInt32().NotNullable()
            .WithColumn("average_lesson_duration_minutes").AsInt32().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("updated_at").AsDateTime().Nullable();

        Create.Table("teaching_subjects")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("tutor_id").AsGuid().NotNullable()
            .WithColumn("subject_id").AsGuid().NotNullable()
            .WithColumn("price_per_hour").AsDecimal(10, 2).NotNullable()
            .WithColumn("description").AsString(500).Nullable()
            .WithColumn("experience_years").AsInt32().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("updated_at").AsDateTime().Nullable();

        Create.Index("ix_teaching_subjects_tutor_subject")
            .OnTable("teaching_subjects")
            .OnColumn("tutor_id").Ascending()
            .OnColumn("subject_id").Ascending()
            .WithOptions().Unique();

        Create.ForeignKey("fk_teaching_subjects_tutor")
            .FromTable("teaching_subjects").ForeignColumn("tutor_id")
            .ToTable("tutors").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.ForeignKey("fk_teaching_subjects_subject")
            .FromTable("teaching_subjects").ForeignColumn("subject_id")
            .ToTable("subjects").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.Table("schedule_slots")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("tutor_id").AsGuid().NotNullable()
            .WithColumn("start_time").AsDateTime().NotNullable()
            .WithColumn("end_time").AsDateTime().NotNullable()
            .WithColumn("status").AsInt32().NotNullable()
            .WithColumn("booking_id").AsGuid().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("updated_at").AsDateTime().Nullable();

        Create.Index("ix_schedule_slots_tutor_time")
            .OnTable("schedule_slots")
            .OnColumn("tutor_id").Ascending()
            .OnColumn("start_time").Ascending()
            .WithOptions().Unique();

        Create.Index("ix_schedule_slots_status")
            .OnTable("schedule_slots")
            .OnColumn("status").Ascending();

        Create.ForeignKey("fk_schedule_slots_tutor")
            .FromTable("schedule_slots").ForeignColumn("tutor_id")
            .ToTable("tutors").PrimaryColumn("id")
            .OnDelete(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("schedule_slots");
        Delete.Table("teaching_subjects");
        Delete.Table("tutors");
        Delete.Table("subjects");
    }
}