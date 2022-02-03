using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    public partial class ComentarioUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "comentarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_UsuarioId",
                table: "comentarios",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_comentarios_AspNetUsers_UsuarioId",
                table: "comentarios",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comentarios_AspNetUsers_UsuarioId",
                table: "comentarios");

            migrationBuilder.DropIndex(
                name: "IX_comentarios_UsuarioId",
                table: "comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "comentarios");
        }
    }
}
