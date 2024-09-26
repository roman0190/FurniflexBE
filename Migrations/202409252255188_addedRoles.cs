namespace FurniflexBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedRoles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "RoleId", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "Category_CategoryId", c => c.Int());
            CreateIndex("dbo.Users", "Category_CategoryId");
            AddForeignKey("dbo.Users", "Category_CategoryId", "dbo.Categories", "CategoryId");
            DropColumn("dbo.Users", "Role");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Role", c => c.String(nullable: false, maxLength: 50));
            DropForeignKey("dbo.Users", "Category_CategoryId", "dbo.Categories");
            DropIndex("dbo.Users", new[] { "Category_CategoryId" });
            DropColumn("dbo.Users", "Category_CategoryId");
            DropColumn("dbo.Users", "RoleId");
        }
    }
}
