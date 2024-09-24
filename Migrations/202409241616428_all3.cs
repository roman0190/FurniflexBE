namespace FurniflexBE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class all3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "ProfilePicture", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "ProfilePicture", c => c.String(maxLength: 255));
        }
    }
}
