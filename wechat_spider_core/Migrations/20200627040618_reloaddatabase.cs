using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace wechat_spider_core.Migrations
{
    public partial class reloaddatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "task_start_sign",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 128, nullable: false),
                    start_date = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_start_sign", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wechat_account",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nick_name = table.Column<string>(maxLength: 128, nullable: false),
                    alias = table.Column<string>(maxLength: 128, nullable: true),
                    fackid = table.Column<string>(maxLength: 128, nullable: true),
                    round_head_img = table.Column<string>(maxLength: 500, nullable: true),
                    service_type = table.Column<int>(nullable: false),
                    last_update_time = table.Column<DateTime>(nullable: true),
                    hometownid = table.Column<string>(maxLength: 50, nullable: true),
                    spider_role = table.Column<string>(maxLength: 20, nullable: true),
                    spider_role1 = table.Column<string>(maxLength: 20, nullable: true),
                    spider_role2 = table.Column<string>(maxLength: 20, nullable: true),
                    spider_role3 = table.Column<string>(maxLength: 20, nullable: true),
                    spider_role4 = table.Column<string>(maxLength: 20, nullable: true),
                    spider_role5 = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wechat_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wechat_article",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 128, nullable: false),
                    create_date = table.Column<DateTime>(nullable: false),
                    hometownid = table.Column<string>(maxLength: 50, nullable: true),
                    download = table.Column<bool>(nullable: false),
                    local_path = table.Column<string>(maxLength: 255, nullable: true),
                    aid = table.Column<string>(nullable: true),
                    album_id = table.Column<string>(nullable: true),
                    appmsgid = table.Column<long>(nullable: false),
                    checking = table.Column<int>(nullable: false),
                    copyright_type = table.Column<int>(nullable: false),
                    cover = table.Column<string>(nullable: true),
                    create_time = table.Column<DateTime>(nullable: false),
                    digest = table.Column<string>(nullable: true),
                    has_red_packet_cover = table.Column<int>(nullable: false),
                    is_original = table.Column<int>(nullable: false),
                    is_pay_subscribe = table.Column<int>(nullable: false),
                    item_show_type = table.Column<int>(nullable: false),
                    itemidx = table.Column<int>(nullable: false),
                    link = table.Column<string>(nullable: true),
                    media_duration = table.Column<string>(nullable: true),
                    mediaapi_publish_status = table.Column<int>(nullable: false),
                    tagid = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    update_time = table.Column<DateTime>(nullable: false),
                    account_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wechat_article", x => x.id);
                    table.ForeignKey(
                        name: "FK_wechat_article_wechat_account_account_id",
                        column: x => x.account_id,
                        principalTable: "wechat_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wechat_article_account_id",
                table: "wechat_article",
                column: "account_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task_start_sign");

            migrationBuilder.DropTable(
                name: "wechat_article");

            migrationBuilder.DropTable(
                name: "wechat_account");
        }
    }
}
