using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cars.Infrastructure.Migrations;

/// <inheritdoc />
public partial class SeedServiceData : Migration
{
    private DateTime now = DateTime.UtcNow;

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            schema: "Cars",
            table: "Clients",
            type: "bit",
            nullable: false,
            defaultValue: false);

        InsertServices(
            migrationBuilder,
            1,
            "Maintenance for petrol car",
            "Change oil, oil filter, air filter, cabine filter",
            299.99m);

        InsertServices(
            migrationBuilder,
            2,
            "Maintenance for diesel car",
            "Change oil, oil filter, fuel filter, air filter, cabine filter",
            399.99m);

        InsertServices(
            migrationBuilder,
            3,
            "Car inspection",
            "Check wheel allignment, leaks in engine and gearbox, lights positioning, brakes and parking brake",
            159.99m);

        InsertServices(
            migrationBuilder,
            4,
            "Timing chain/belt change",
            "Change timing chain/belt, tensioners and gears",
            1000.00m);

        InsertServices(
            migrationBuilder,
            5,
            "Full brakes change",
            "Change brake pads and discs on all wheels",
            699.99m);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }

    private void InsertServices(MigrationBuilder migrationBuilder, int id, string serviceName, string serviceDescription, decimal price)
    {
        migrationBuilder.InsertData(
            schema: "Cars",
            table: "Services",
            columns: new[]
            {
                "Id", "ServiceName", "ServiceDescription", "Price",
                "ServiceDate", "CreatedOn", "ModifiedOn"
            },
            values: new object[]
            {
                id, serviceName, serviceDescription, price, now, now, now
            });
    }
}
