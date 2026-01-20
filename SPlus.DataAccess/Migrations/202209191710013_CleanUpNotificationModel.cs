namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CleanUpNotificationModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "RelatedItemID", c => c.Int(nullable: false));
            AlterColumn("dbo.Notifications", "EnglishName", c => c.String(nullable: false));
            AlterColumn("dbo.Notifications", "ArabicName", c => c.String(nullable: false));
            AlterColumn("dbo.Notifications", "AssignedTo", c => c.String(nullable: false));
            AlterColumn("dbo.Notifications", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.Notifications", "NotificationType", c => c.String(nullable: false));
            AlterColumn("dbo.Notifications", "Modified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Notifications", "Created", c => c.DateTime(nullable: false));
            DropColumn("dbo.Notifications", "KPIID");
            DropColumn("dbo.Notifications", "DelegationID");
            DropColumn("dbo.Notifications", "KPIName");
            DropColumn("dbo.Notifications", "TaskID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "TaskID", c => c.Decimal(precision: 18, scale: 0));
            AddColumn("dbo.Notifications", "KPIName", c => c.String(maxLength: 255));
            AddColumn("dbo.Notifications", "DelegationID", c => c.Decimal(precision: 18, scale: 0));
            AddColumn("dbo.Notifications", "KPIID", c => c.Decimal(precision: 18, scale: 0));
            AlterColumn("dbo.Notifications", "Created", c => c.DateTime());
            AlterColumn("dbo.Notifications", "Modified", c => c.DateTime());
            AlterColumn("dbo.Notifications", "NotificationType", c => c.String(maxLength: 255));
            AlterColumn("dbo.Notifications", "Status", c => c.String(maxLength: 255));
            AlterColumn("dbo.Notifications", "AssignedTo", c => c.String(maxLength: 255));
            AlterColumn("dbo.Notifications", "ArabicName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Notifications", "EnglishName", c => c.String(maxLength: 255));
            DropColumn("dbo.Notifications", "RelatedItemID");
        }
    }
}
