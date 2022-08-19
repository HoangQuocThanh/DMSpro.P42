using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMSpro.P42.Migrations
{
    public partial class Add_CompanyIdentityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MDMCompanyIdentityUser",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MDMCompanyIdentityUser", x => new { x.CompanyId, x.IdentityUserId });
                    table.ForeignKey(
                        name: "FK_MDMCompanyIdentityUser_AbpUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MDMCompanyIdentityUser_MDMCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "MDMCompanies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MDMCompanyIdentityUser_CompanyId_IdentityUserId",
                table: "MDMCompanyIdentityUser",
                columns: new[] { "CompanyId", "IdentityUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_MDMCompanyIdentityUser_IdentityUserId",
                table: "MDMCompanyIdentityUser",
                column: "IdentityUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MDMCompanyIdentityUser");
        }
    }
}
