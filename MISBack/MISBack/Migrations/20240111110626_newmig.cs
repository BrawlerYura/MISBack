using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MISBack.Migrations
{
    /// <inheritdoc />
    public partial class newmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Consultation_ConsultationId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation_Speciality_SpecialityId",
                table: "Consultation");

            migrationBuilder.DropIndex(
                name: "IX_Consultation_SpecialityId",
                table: "Consultation");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ConsultationId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "IsWithDiagnosis",
                table: "Inspection");

            migrationBuilder.DropColumn(
                name: "InspectionId",
                table: "Diagnosis");

            migrationBuilder.RenameColumn(
                name: "Patient",
                table: "Inspection",
                newName: "PatientId");

            migrationBuilder.AlterColumn<Guid>(
                name: "DoctorId",
                table: "Inspection",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRootComment",
                table: "Comment",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "InspectionDiagnosis",
                columns: table => new
                {
                    InspectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiagnosisId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionDiagnosis", x => new { x.InspectionId, x.DiagnosisId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionDiagnosis");

            migrationBuilder.DropColumn(
                name: "IsRootComment",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Inspection",
                newName: "Patient");

            migrationBuilder.AlterColumn<Guid>(
                name: "DoctorId",
                table: "Inspection",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "IsWithDiagnosis",
                table: "Inspection",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "InspectionId",
                table: "Diagnosis",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_SpecialityId",
                table: "Consultation",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ConsultationId",
                table: "Comment",
                column: "ConsultationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Consultation_ConsultationId",
                table: "Comment",
                column: "ConsultationId",
                principalTable: "Consultation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation_Speciality_SpecialityId",
                table: "Consultation",
                column: "SpecialityId",
                principalTable: "Speciality",
                principalColumn: "Id");
        }
    }
}
