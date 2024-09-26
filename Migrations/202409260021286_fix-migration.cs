namespace FurniflexBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixmigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Category_CategoryId", "dbo.Categories");
            DropIndex("dbo.Users", new[] { "Category_CategoryId" });
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateIndex("dbo.Users", "RoleId");
            AddForeignKey("dbo.Users", "RoleId", "dbo.Roles", "RoleId", cascadeDelete: true);
            DropColumn("dbo.Users", "Category_CategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Category_CategoryId", c => c.Int());
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropTable("dbo.Roles");
            CreateIndex("dbo.Users", "Category_CategoryId");
            AddForeignKey("dbo.Users", "Category_CategoryId", "dbo.Categories", "CategoryId");
        }
    }
}
