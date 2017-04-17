using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAPI.Migrations
{
    public partial class RateRouteMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookmarks",
                columns: table => new
                {
                    SSBookmarkId = table.Column<string>(nullable: false),
                    ItemId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmarks", x => x.SSBookmarkId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    SSGroupId = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    AdminId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    ItemsSold = table.Column<int>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    LocationDetail = table.Column<string>(nullable: true),
                    Longitude = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    Routing = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.SSGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ItemId = table.Column<string>(nullable: false),
                    Thumbnail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ItemId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    SSItemId = table.Column<string>(nullable: false),
                    Approved = table.Column<bool>(nullable: false),
                    Condition = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GroupId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    Price = table.Column<string>(nullable: true),
                    Thumbnail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.SSItemId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    SSMessageId = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    DatePosted = table.Column<string>(nullable: true),
                    ItemId = table.Column<string>(nullable: true),
                    PosterId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.SSMessageId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    SSUserId = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    GroupId = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.SSUserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookmarks");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
