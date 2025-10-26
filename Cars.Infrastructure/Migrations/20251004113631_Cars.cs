using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cars.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Cars : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Cars");

        migrationBuilder.CreateTable(
            name: "Clients",
            schema: "Cars",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                Surname = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Clients", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Services",
            schema: "Cars",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ServiceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                ServiceDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0.00m),
                ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Services", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Cars",
            schema: "Cars",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Make = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Model = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Year = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                VIN = table.Column<string>(type: "nchar(17)", fixedLength: true, maxLength: 17, nullable: false),
                Visits = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                ClientId = table.Column<int>(type: "int", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                Modification = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cars", x => x.Id);
                table.ForeignKey(
                    name: "FK_Cars_Clients_ClientId",
                    column: x => x.ClientId,
                    principalSchema: "Cars",
                    principalTable: "Clients",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Cars_ClientId",
            schema: "Cars",
            table: "Cars",
            column: "ClientId");

        migrationBuilder.CreateIndex(
            name: "IX_Cars_VIN",
            schema: "Cars",
            table: "Cars",
            column: "VIN",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Cars",
            schema: "Cars");

        migrationBuilder.DropTable(
            name: "Services",
            schema: "Cars");

        migrationBuilder.DropTable(
            name: "Clients",
            schema: "Cars");
    }
}
