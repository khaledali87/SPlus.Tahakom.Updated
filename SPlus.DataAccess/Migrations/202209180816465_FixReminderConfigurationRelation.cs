namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixReminderConfigurationRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReminderConfiguration", "KPITypeID", "dbo.KPIType");
            DropIndex("dbo.ReminderConfiguration", new[] { "KPITypeID" });
            AddColumn("dbo.ReminderConfiguration", "KPIType_KPITypeID", c => c.Int(nullable: false));
            CreateIndex("dbo.ReminderConfiguration", "KPIType_KPITypeID");
            AddForeignKey("dbo.ReminderConfiguration", "KPIType_KPITypeID", "dbo.KPIType", "KPITypeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReminderConfiguration", "KPIType_KPITypeID", "dbo.KPIType");
            DropIndex("dbo.ReminderConfiguration", new[] { "KPIType_KPITypeID" });
            DropColumn("dbo.ReminderConfiguration", "KPIType_KPITypeID");
            CreateIndex("dbo.ReminderConfiguration", "KPITypeID");
            AddForeignKey("dbo.ReminderConfiguration", "KPITypeID", "dbo.KPIType", "KPITypeID", cascadeDelete: true);
        }
    }
}
