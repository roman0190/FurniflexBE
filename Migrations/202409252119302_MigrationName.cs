namespace FurniflexBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Location", c => c.String());
            AlterColumn("dbo.Users", "Phone", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Phone", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Location", c => c.String(nullable: false));
        }
    }
}
