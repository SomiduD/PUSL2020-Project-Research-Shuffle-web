using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlindMatch.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentIdToProposals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "ProjectProposals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "ProjectProposals");
        }
    }
}
