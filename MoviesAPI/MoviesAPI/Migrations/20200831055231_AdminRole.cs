using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Insert into AspNetRoles (Id, [Name], [NormalizedName])
            values ('03e91044-27ae-47a8-bafd-e8c6b12bb4be', 'Admin', 'Admin')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete AspNetRoles where id = '03e91044-27ae-47a8-bafd-e8c6b12bb4be'
            ");
        }
    }
}
