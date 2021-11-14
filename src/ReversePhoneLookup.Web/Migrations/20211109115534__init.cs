using Microsoft.EntityFrameworkCore.Migrations;

namespace ReversePhoneLookup.Web.Migrations
{
    public partial class _init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "contact_id_seq");

            migrationBuilder.CreateSequence(
                name: "operator_id_seq");

            migrationBuilder.CreateSequence(
                name: "phone_id_seq");

            migrationBuilder.CreateTable(
                name: "operator",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mcc = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    mnc = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operator", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "phone",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    operator_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone", x => x.id);
                    table.ForeignKey(
                        name: "phone_operator_id_fkey",
                        column: x => x.operator_id,
                        principalTable: "operator",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    phone_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact", x => x.id);
                    table.ForeignKey(
                        name: "contact_phone_id_fkey",
                        column: x => x.phone_id,
                        principalTable: "phone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contact_phone_id",
                table: "contact",
                column: "phone_id");

            migrationBuilder.CreateIndex(
                name: "IX_phone_operator_id",
                table: "phone",
                column: "operator_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "phone");

            migrationBuilder.DropTable(
                name: "operator");

            migrationBuilder.DropSequence(
                name: "contact_id_seq");

            migrationBuilder.DropSequence(
                name: "operator_id_seq");

            migrationBuilder.DropSequence(
                name: "phone_id_seq");
        }
    }
}
