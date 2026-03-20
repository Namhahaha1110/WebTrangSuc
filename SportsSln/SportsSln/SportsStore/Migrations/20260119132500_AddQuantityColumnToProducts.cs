using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsStore.Migrations
{
    public partial class AddQuantityColumnToProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Products') AND name = 'Quantity')
                BEGIN
                    ALTER TABLE [Products] ADD [Quantity] int NOT NULL DEFAULT 0
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Products') AND name = 'Quantity')
                BEGIN
                    ALTER TABLE [Products] DROP COLUMN [Quantity]
                END
            ");
        }
    }
}
