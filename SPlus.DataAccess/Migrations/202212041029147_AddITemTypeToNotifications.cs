namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddITemTypeToNotifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "ItemType", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "ItemType");
        }
    }
}
