using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class testForiegnKeys333eeeww22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

           

            migrationBuilder.AddColumn<int>(
                name: "AppTenantId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppTenantId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_AppTenantId",
                table: "Products",
                column: "AppTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AppTenantId",
                table: "AspNetUsers",
                column: "AppTenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tenants_AppTenantId",
                table: "AspNetUsers",
                column: "AppTenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tenants_AppTenantId",
                table: "Products",
                column: "AppTenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tenants_AppTenantId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Tenants_AppTenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_AppTenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AppTenantId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppTenantId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AppTenantId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Tenants_TenantId",
                table: "Products",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
