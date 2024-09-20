namespace FurniflexBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class all : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Order_OrderId = c.Int(),
                    })
                .PrimaryKey(t => t.CartId)
                .ForeignKey("dbo.Orders", t => t.Order_OrderId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProductId)
                .Index(t => t.Order_OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderStatus = c.String(nullable: false, maxLength: 50),
                        PaymentStatus = c.String(nullable: false, maxLength: 255),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                        ProfilePicture = c.String(maxLength: 255),
                        Role = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(maxLength: 1000),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        ImgUrl = c.String(nullable: false, maxLength: 255),
                        CategoryId = c.Int(nullable: false),
                        DiscountedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(nullable: false, maxLength: 1000),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        OrderItemId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Carts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Reviews", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reviews", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Carts", "UserId", "dbo.Users");
            DropForeignKey("dbo.Carts", "Order_OrderId", "dbo.Orders");
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.Reviews", new[] { "ProductId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Carts", new[] { "Order_OrderId" });
            DropIndex("dbo.Carts", new[] { "ProductId" });
            DropIndex("dbo.Carts", new[] { "UserId" });
            DropTable("dbo.OrderItems");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.Reviews");
            DropTable("dbo.Users");
            DropTable("dbo.Orders");
            DropTable("dbo.Carts");
        }
    }
}
