namespace FurniflexBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class all1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "location", c => c.String());
            AddColumn("dbo.Users", "phone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "phone");
            DropColumn("dbo.Users", "location");
        }
    }
}
