using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMSystem.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessCode = table.Column<string>(type: "text", nullable: true),
                    ProcessConfig = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueryConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uuid", nullable: false),
                    JsonData = table.Column<string>(type: "text", nullable: true),
                    StepId = table.Column<string>(type: "text", nullable: true),
                    ProcessCode = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateContainerConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessCode = table.Column<string>(type: "text", nullable: true),
                    JsonData = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedDate = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedOn = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateContainerConfigs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Processes");

            migrationBuilder.DropTable(
                name: "QueryConfigs");

            migrationBuilder.DropTable(
                name: "StateContainerConfigs");
        }
    }
}
