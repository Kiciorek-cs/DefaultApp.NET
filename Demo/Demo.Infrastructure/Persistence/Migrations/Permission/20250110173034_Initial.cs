using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Demo.Infrastructure.Persistence.Migrations.Permission;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Resources",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>("nvarchar(max)", nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: false),
                HttpMethod = table.Column<string>("nvarchar(max)", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Resources", x => x.Id); });

        migrationBuilder.CreateTable(
            "Roles",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>("nvarchar(max)", nullable: false),
                Description = table.Column<string>("nvarchar(max)", nullable: false),
                IsDefaultRole = table.Column<bool>("bit", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Roles", x => x.Id); });

        migrationBuilder.CreateTable(
            "ResourceRole",
            table => new
            {
                ResourcesId = table.Column<int>("int", nullable: false),
                RolesId = table.Column<int>("int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ResourceRole", x => new { x.ResourcesId, x.RolesId });
                table.ForeignKey(
                    "FK_ResourceRole_Resources_ResourcesId",
                    x => x.ResourcesId,
                    "Resources",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_ResourceRole_Roles_RolesId",
                    x => x.RolesId,
                    "Roles",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Users",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>("nvarchar(max)", nullable: false),
                FirstNameUnified = table.Column<string>("nvarchar(max)", nullable: false),
                LastName = table.Column<string>("nvarchar(max)", nullable: false),
                LastNameUnified = table.Column<string>("nvarchar(max)", nullable: false),
                Gender = table.Column<string>("nvarchar(max)", nullable: false),
                DateOfBirth = table.Column<DateOnly>("date", nullable: false),
                RoleId = table.Column<int>("int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    "FK_Users_Roles_RoleId",
                    x => x.RoleId,
                    "Roles",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            "Logins",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LoginName = table.Column<string>("nvarchar(max)", nullable: false),
                PasswordHash = table.Column<string>("nvarchar(max)", nullable: false),
                PasswordSalt = table.Column<string>("nvarchar(max)", nullable: false),
                EmailAddress = table.Column<string>("nvarchar(max)", nullable: false),
                TokenGenerationTime = table.Column<int>("int", nullable: false),
                EmailValidationStatus = table.Column<string>("nvarchar(max)", nullable: false),
                Status = table.Column<string>("nvarchar(max)", nullable: false),
                UserId = table.Column<int>("int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Logins", x => x.Id);
                table.ForeignKey(
                    "FK_Logins_Users_UserId",
                    x => x.UserId,
                    "Users",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Tokens",
            table => new
            {
                Id = table.Column<int>("int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TokenType = table.Column<string>("nvarchar(max)", nullable: false),
                StatusType = table.Column<string>("nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>("datetimeoffset", nullable: false),
                ExpirationDate = table.Column<DateTimeOffset>("datetimeoffset", nullable: true),
                Value = table.Column<string>("nvarchar(max)", nullable: false),
                LoginId = table.Column<int>("int", nullable: false),
                ParentTokenId = table.Column<int>("int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tokens", x => x.Id);
                table.ForeignKey(
                    "FK_Tokens_Logins_LoginId",
                    x => x.LoginId,
                    "Logins",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_Tokens_Tokens_ParentTokenId",
                    x => x.ParentTokenId,
                    "Tokens",
                    "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            "IX_Logins_UserId",
            "Logins",
            "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_ResourceRole_RolesId",
            "ResourceRole",
            "RolesId");

        migrationBuilder.CreateIndex(
            "IX_Tokens_LoginId",
            "Tokens",
            "LoginId");

        migrationBuilder.CreateIndex(
            "IX_Tokens_ParentTokenId",
            "Tokens",
            "ParentTokenId");

        migrationBuilder.CreateIndex(
            "IX_Users_RoleId",
            "Users",
            "RoleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "ResourceRole");

        migrationBuilder.DropTable(
            "Tokens");

        migrationBuilder.DropTable(
            "Resources");

        migrationBuilder.DropTable(
            "Logins");

        migrationBuilder.DropTable(
            "Users");

        migrationBuilder.DropTable(
            "Roles");
    }
}