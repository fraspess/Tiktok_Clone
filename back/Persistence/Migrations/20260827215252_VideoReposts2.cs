using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VideoReposts2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoRepostEntity_AspNetUsers_UserId",
                table: "VideoRepostEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoRepostEntity_Videos_VideoId",
                table: "VideoRepostEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoRepostEntity",
                table: "VideoRepostEntity");

            migrationBuilder.RenameTable(
                name: "VideoRepostEntity",
                newName: "VideoReposts");

            migrationBuilder.RenameIndex(
                name: "IX_VideoRepostEntity_UserId",
                table: "VideoReposts",
                newName: "IX_VideoReposts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoReposts",
                table: "VideoReposts",
                columns: new[] { "VideoId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_VideoReposts_AspNetUsers_UserId",
                table: "VideoReposts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoReposts_Videos_VideoId",
                table: "VideoReposts",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoReposts_AspNetUsers_UserId",
                table: "VideoReposts");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoReposts_Videos_VideoId",
                table: "VideoReposts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoReposts",
                table: "VideoReposts");

            migrationBuilder.RenameTable(
                name: "VideoReposts",
                newName: "VideoRepostEntity");

            migrationBuilder.RenameIndex(
                name: "IX_VideoReposts_UserId",
                table: "VideoRepostEntity",
                newName: "IX_VideoRepostEntity_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoRepostEntity",
                table: "VideoRepostEntity",
                columns: new[] { "VideoId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_VideoRepostEntity_AspNetUsers_UserId",
                table: "VideoRepostEntity",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoRepostEntity_Videos_VideoId",
                table: "VideoRepostEntity",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
