using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduAsistant.Migrations
{
    /// <inheritdoc />
    public partial class SylabbusFileNameSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SyllabusFileName",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyllabusFileName",
                table: "Subjects");
        }
    }
}
