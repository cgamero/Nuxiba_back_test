using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBackNuxiba.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ccRIACat_Areas",
                columns: table => new
                {
                    IDArea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatusArea = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccRIACat_Areas", x => x.IDArea);
                });

            migrationBuilder.CreateTable(
                name: "ccUsers",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TipoUser_id = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    fCreate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IDArea = table.Column<int>(type: "int", nullable: true),
                    LastLoginAttempt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccUsers", x => x.User_id);
                    table.ForeignKey(
                        name: "FK_ccUsers_ccRIACat_Areas_IDArea",
                        column: x => x.IDArea,
                        principalTable: "ccRIACat_Areas",
                        principalColumn: "IDArea",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ccloglogin",
                columns: table => new
                {
                    LogLoginId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_id = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<int>(type: "int", nullable: false),
                    TipoMov = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccloglogin", x => x.LogLoginId);
                    table.CheckConstraint("CK_ccloglogin_TipoMov", "[TipoMov] IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_ccloglogin_ccUsers_User_id",
                        column: x => x.User_id,
                        principalTable: "ccUsers",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ccloglogin_User_id_fecha",
                table: "ccloglogin",
                columns: new[] { "User_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_ccloglogin_User_id_TipoMov_fecha",
                table: "ccloglogin",
                columns: new[] { "User_id", "TipoMov", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_ccUsers_IDArea",
                table: "ccUsers",
                column: "IDArea");

            migrationBuilder.CreateIndex(
                name: "IX_ccUsers_Login",
                table: "ccUsers",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ccloglogin");

            migrationBuilder.DropTable(
                name: "ccUsers");

            migrationBuilder.DropTable(
                name: "ccRIACat_Areas");
        }
    }
}
