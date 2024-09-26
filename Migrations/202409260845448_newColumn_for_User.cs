namespace FurniflexBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newColumn_for_User : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Key", c => c.String());
            AddColumn("dbo.Users", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CreatedAt");
            DropColumn("dbo.Users", "Key");
        }
    }
}
