namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupNotificationConfigurationAndReminderRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailTemplate", "Type", c => c.String());
            AddColumn("dbo.EmailTemplate", "To", c => c.String());
            AddColumn("dbo.NotificationReceivers", "IsGroup", c => c.Boolean(nullable: false));
            AddColumn("dbo.ReminderConfiguration", "KPITypeID", c => c.Int(nullable: false));
            AlterColumn("dbo.EmailConfiguration", "SmtpPort", c => c.Int(nullable: false));
            AlterColumn("dbo.EmailConfiguration", "EnableSsl", c => c.Boolean(nullable: false));
            AlterColumn("dbo.EmailConfiguration", "UsingSharepointSMTP", c => c.Boolean(nullable: false));
            AlterColumn("dbo.NotificationReceivers", "Receiver", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.NotificationReceivers", "IsCC", c => c.Boolean(nullable: false));
            CreateIndex("dbo.ReminderConfiguration", "KPITypeID");
            AddForeignKey("dbo.ReminderConfiguration", "KPITypeID", "dbo.KPIType", "KPITypeID", cascadeDelete: true);
            DropColumn("dbo.ReminderConfiguration", "TypeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReminderConfiguration", "TypeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.ReminderConfiguration", "KPITypeID", "dbo.KPIType");
            DropIndex("dbo.ReminderConfiguration", new[] { "KPITypeID" });
            AlterColumn("dbo.NotificationReceivers", "IsCC", c => c.Boolean());
            AlterColumn("dbo.NotificationReceivers", "Receiver", c => c.String(maxLength: 255));
            AlterColumn("dbo.EmailConfiguration", "UsingSharepointSMTP", c => c.Boolean());
            AlterColumn("dbo.EmailConfiguration", "EnableSsl", c => c.Boolean());
            AlterColumn("dbo.EmailConfiguration", "SmtpPort", c => c.String(maxLength: 255));
            DropColumn("dbo.ReminderConfiguration", "KPITypeID");
            DropColumn("dbo.NotificationReceivers", "IsGroup");
            DropColumn("dbo.EmailTemplate", "To");
            DropColumn("dbo.EmailTemplate", "Type");
        }
    }
}
