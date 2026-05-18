using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduAsistant.Migrations
{
    /// <inheritdoc />
    public partial class AddFigmaFieldsToSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "DailyStudyLimit",
                table: "Subjects",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DailyStudyLimit",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Subjects");
        }
    }
}
