using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewBuilder.Migrations
{
    public partial class Mmm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserModel",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    builded = table.Column<bool>(nullable: false),
                    dt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    UserId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileData_UserModel_UserId1",
                        column: x => x.UserId1,
                        principalTable: "UserModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileData_UserModel_UserId",
                        column: x => x.UserId,
                        principalTable: "UserModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Discipline = table.Column<string>(nullable: true),
                    Theme = table.Column<string>(nullable: true),
                    StudentName = table.Column<string>(nullable: true),
                    StudentGroup = table.Column<string>(nullable: true),
                    ChiefName = table.Column<string>(nullable: true),
                    FieldFileDataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewFields_FileData_FieldFileDataId",
                        column: x => x.FieldFileDataId,
                        principalTable: "FileData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReviewFieldsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluation_ReviewFields_ReviewFieldsId",
                        column: x => x.ReviewFieldsId,
                        principalTable: "ReviewFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluation_ReviewFieldsId",
                table: "Evaluation",
                column: "ReviewFieldsId");

            migrationBuilder.CreateIndex(
                name: "IX_FileData_UserId1",
                table: "FileData",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FileData_UserId",
                table: "FileData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewFields_FieldFileDataId",
                table: "ReviewFields",
                column: "FieldFileDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evaluation");

            migrationBuilder.DropTable(
                name: "ReviewFields");

            migrationBuilder.DropTable(
                name: "FileData");

            migrationBuilder.DropTable(
                name: "UserModel");
        }
    }
}
