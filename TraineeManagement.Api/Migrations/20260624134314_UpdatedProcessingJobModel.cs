using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProcessingJobModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedDate",
                table: "ProcessingJobs",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "CompletedDate",
                table: "ProcessingJobs",
                newName: "CompletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "ProcessingJobs",
                newName: "StartedDate");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "ProcessingJobs",
                newName: "CompletedDate");
        }
    }
}
