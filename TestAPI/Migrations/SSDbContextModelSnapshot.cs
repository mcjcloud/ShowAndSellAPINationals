using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ShowAndSellAPI.Models.Database;

namespace TestAPI.Migrations
{
    [DbContext(typeof(SSDbContext))]
    partial class SSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ShowAndSellAPI.Models.SSBookmark", b =>
                {
                    b.Property<string>("SSBookmarkId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ItemId");

                    b.Property<string>("UserId");

                    b.HasKey("SSBookmarkId");

                    b.ToTable("Bookmarks");
                });

            modelBuilder.Entity("ShowAndSellAPI.Models.SSGroup", b =>
                {
                    b.Property<string>("SSGroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("AdminId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("ItemsSold");

                    b.Property<double>("Latitude");

                    b.Property<string>("LocationDetail");

                    b.Property<double>("Longitude");

                    b.Property<string>("Name");

                    b.Property<float>("Rating");

                    b.Property<string>("Routing");

                    b.HasKey("SSGroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("ShowAndSellAPI.Models.SSImage", b =>
                {
                    b.Property<string>("ItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Thumbnail");

                    b.HasKey("ItemId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ShowAndSellAPI.Models.SSItem", b =>
                {
                    b.Property<string>("SSItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Approved");

                    b.Property<string>("Condition");

                    b.Property<string>("Description");

                    b.Property<string>("GroupId");

                    b.Property<string>("Name");

                    b.Property<string>("OwnerId");

                    b.Property<string>("Price");

                    b.Property<string>("Thumbnail");

                    b.HasKey("SSItemId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("ShowAndSellAPI.Models.SSMessage", b =>
                {
                    b.Property<string>("SSMessageId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<string>("DatePosted");

                    b.Property<string>("ItemId");

                    b.Property<string>("PosterId");

                    b.HasKey("SSMessageId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("ShowAndSellAPI.Models.SSRating", b =>
                {
                    b.Property<string>("SSRatingId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Rating");

                    b.Property<string>("UserId");

                    b.HasKey("SSRatingId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("ShowAndSellAPI.Models.SSUser", b =>
                {
                    b.Property<string>("SSUserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("GroupId");

                    b.Property<string>("LastName");

                    b.Property<string>("Password");

                    b.HasKey("SSUserId");

                    b.ToTable("Users");
                });
        }
    }
}
