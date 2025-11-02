using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KaliteKontrol.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADIM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TIP_KODU = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    HAT_ADI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IST_ADI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IST_NO = table.Column<int>(type: "int", nullable: false),
                    ADIM_NO = table.Column<int>(type: "int", nullable: false),
                    ISLEM_NO = table.Column<int>(type: "int", nullable: false),
                    ISLEM_ADI = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BC_TANIM = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SIKICI_NO = table.Column<int>(type: "int", nullable: false),
                    PROG_NO = table.Column<int>(type: "int", nullable: false),
                    SIKIM_ADEDI = table.Column<int>(type: "int", nullable: false),
                    REWORK_SIKICI_NO = table.Column<int>(type: "int", nullable: false),
                    REWORK_PROG_NO = table.Column<int>(type: "int", nullable: false),
                    SURE = table.Column<int>(type: "int", nullable: false),
                    RESIM_YOLU = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ETIKET = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADIM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ISLEMLER",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISLEM_NO = table.Column<int>(type: "int", nullable: false),
                    ISLEM_ADI = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ISLEMLER", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Yetki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YetkiAdi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yetki", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    YetkiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Yetki_YetkiId",
                        column: x => x.YetkiId,
                        principalTable: "Yetki",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ISLEMLER_ISLEM_NO",
                table: "ISLEMLER",
                column: "ISLEM_NO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_YetkiId",
                table: "Kullanicilar",
                column: "YetkiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADIM");

            migrationBuilder.DropTable(
                name: "ISLEMLER");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Yetki");
        }
    }
}
