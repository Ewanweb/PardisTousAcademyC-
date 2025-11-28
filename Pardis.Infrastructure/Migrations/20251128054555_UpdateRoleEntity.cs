using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pardis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1bf6dba6-60f0-456d-b31f-d8d061662df2", "AQAAAAIAAYagAAAAEMlQqPorB+LqtGhxIme2AosqN4vSZBXLvFxSyvS2rm/mYrxDinDC9y7YUyUUhCwPaA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "61dea609-9f56-4fd3-a894-897bb7062d9b", "AQAAAAIAAYagAAAAEFozZktfkHDsiuOyfTVXPTRW1BwmaEIW8vfxO1h2icnH2S/Gax6c/QDfFT9EH/GFNQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c4e6097-f570-4927-b2f7-5f65d1373555",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "464043b1-7372-46c2-9a7b-76026724be1c", "AQAAAAIAAYagAAAAEGBMBFNzlCpeebHsq8zfLA0fIYx9fDlL5RBCA3manWLkmlEh0K7kqwP0s5lVRkeX+g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5117a984-3399-43f4-b7c7-5b7cb11ba558", "AQAAAAIAAYagAAAAEMorXuBKMB6vasAAZABvhB3SIES0dXY7iiP7xdzXi3i7D+7B028qySvv/fnxZo9bcA==" });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "00edafe3-b047-5980-d0fa-da10f400c1e5", "fecf871f-83ad-49f6-afc2-c65793b96cc6", "Admin", "ADMIN" },
                    { "19d55d6e-3f29-3955-a536-31adce03c0b2", "9019f580-19a1-4d07-9283-becd059eb166", "ITManager", "ITMANAGER" },
                    { "2c131675-e751-e65c-5534-cbdcc96c2c45", "81c6a805-b3e4-47cd-bd46-f5c77af5effc", "Accountant", "ACCOUNTANT" },
                    { "3cbe94ae-32d5-4ace-0258-84819eb08c98", "c350b5a5-993a-4b02-a9be-39c471888b83", "Manager", "MANAGER" },
                    { "52c145a7-4624-5c8d-f270-e6a048c2ca71", "2c2420be-15b1-4f1f-9a1e-b2e0b91c336c", "EducationExpert", "EDUCATIONEXPERT" },
                    { "660905f4-03e4-eec3-85e1-735cab1308e8", "5b6c337d-ffd7-4ef3-bf85-236c04924dc9", "EducationManager", "EDUCATIONMANAGER" },
                    { "6cc9d6b9-06ae-ceb2-5ff7-bec172fc7cb7", "9d1d40da-c32d-45bb-b313-93ee1272deec", "Marketer", "MARKETER" },
                    { "6ddc93a3-048a-da24-faab-ed198c8d1e5a", "f822f812-4582-42bc-9ac5-cccb66361df2", "CourseSupport", "COURSESUPPORT" },
                    { "9dfe9b8f-4513-7c23-b3b2-b205864da075", "7f1cc765-1dcc-47f0-9006-b1dc98c10906", "User", "USER" },
                    { "a74fab8d-2d81-7ae5-1871-0538e4088d26", "3b305c22-af9e-4022-bab2-983dfcfa2aff", "MarketingManager", "MARKETINGMANAGER" },
                    { "ae0ec1fc-3a9e-6b14-e4b7-b1f23adaf800", "6b9af964-9a80-49fa-94bd-8954ca895155", "GeneralManager", "GENERALMANAGER" },
                    { "bdbda483-9107-7444-1c63-b8f8b8f56aff", "f97613f2-a891-4867-a375-9d46e90b675f", "InternalManager", "INTERNALMANAGER" },
                    { "c6416b99-2025-9adc-1585-61825436c565", "ff367eff-9198-4a06-840d-a7a7b38c64b5", "FinancialManager", "FINANCIALMANAGER" },
                    { "c8bcf807-3aac-0148-1c67-c0d0f77f07d0", "8ef0acda-511b-4533-9128-fc7fe421564d", "Instructor", "INSTRUCTOR" },
                    { "c9a1c0f5-4c38-252e-e79b-a1abf5d9a037", "79abfb6f-36ec-4f9f-854e-98ba9c08aa19", "Student", "STUDENT" },
                    { "ce39a444-665b-6b66-23aa-fede411c6fc0", "81c9b7ae-b69f-4e95-be2b-da6617a37702", "DepartmentManager", "DEPARTMENTMANAGER" }
                });
        }
    }
}
