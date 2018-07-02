using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewBuilder.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    builded = table.Column<bool>(nullable: false),
                    dt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    UserModelId = table.Column<int>(nullable: true),
                    UserModelId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileModel_UserModel_UserModelId1",
                        column: x => x.UserModelId1,
                        principalTable: "UserModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileModel_UserModel_UserModelId",
                        column: x => x.UserModelId,
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
                    FieldFileModelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewFields_FileModel_FieldFileModelId",
                        column: x => x.FieldFileModelId,
                        principalTable: "FileModel",
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
                name: "IX_FileModel_UserModelId1",
                table: "FileModel",
                column: "UserModelId1");

            migrationBuilder.CreateIndex(
                name: "IX_FileModel_UserModelId",
                table: "FileModel",
                column: "UserModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewFields_FieldFileModelId",
                table: "ReviewFields",
                column: "FieldFileModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evaluation");

            migrationBuilder.DropTable(
                name: "ReviewFields");

            migrationBuilder.DropTable(
                name: "FileModel");

            migrationBuilder.DropTable(
                name: "UserModel");
        }
    }
}
