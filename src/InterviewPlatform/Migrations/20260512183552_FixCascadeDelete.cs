using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewPlatform.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttempts_Users_UserId",
                table: "CourseAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_CreatorId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAttempts_Questions_QuestionId",
                table: "QuestionAttempts");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttempts_Users_UserId",
                table: "CourseAttempts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_CreatorId",
                table: "Courses",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAttempts_Questions_QuestionId",
                table: "QuestionAttempts",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttempts_Users_UserId",
                table: "CourseAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_CreatorId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAttempts_Questions_QuestionId",
                table: "QuestionAttempts");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttempts_Users_UserId",
                table: "CourseAttempts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_CreatorId",
                table: "Courses",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAttempts_Questions_QuestionId",
                table: "QuestionAttempts",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
