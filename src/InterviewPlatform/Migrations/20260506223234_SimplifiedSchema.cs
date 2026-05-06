using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewPlatform.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Interviews_InterviewId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "AnswerAttempts");

            migrationBuilder.DropTable(
                name: "InterviewAttempts");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Questions_InterviewId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "InterviewId",
                table: "Questions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InterviewId",
                table: "Questions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    PassingScore = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interviews_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    TraineeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    TotalScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewAttempts_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewAttempts_Users_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnswerAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AiScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AiStrengths = table.Column<string>(type: "text", nullable: false),
                    AiSuggestions = table.Column<string>(type: "text", nullable: false),
                    AiWeaknesses = table.Column<string>(type: "text", nullable: false),
                    SubmittedText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerAttempts_InterviewAttempts_InterviewAttemptId",
                        column: x => x.InterviewAttemptId,
                        principalTable: "InterviewAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerAttempts_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_InterviewId",
                table: "Questions",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerAttempts_InterviewAttemptId",
                table: "AnswerAttempts",
                column: "InterviewAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerAttempts_QuestionId",
                table: "AnswerAttempts",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAttempts_InterviewId",
                table: "InterviewAttempts",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAttempts_TraineeId",
                table: "InterviewAttempts",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_CourseId",
                table: "Interviews",
                column: "CourseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Interviews_InterviewId",
                table: "Questions",
                column: "InterviewId",
                principalTable: "Interviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
