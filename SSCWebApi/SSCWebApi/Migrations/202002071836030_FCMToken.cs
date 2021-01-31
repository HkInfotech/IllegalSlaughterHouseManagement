namespace SSCWebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FCMToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FCMToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FCMToken");
        }
    }
}
