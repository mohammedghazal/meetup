using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.API.Migrations
{
    public partial class teradfa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReporteeId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReporteeId",
                table: "Reports",
                column: "ReporteeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReporteeId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReporteeId",
                table: "Reports",
                column: "ReporteeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
