namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivityTypeAndItRelatedTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityReminderConfiguration",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ActivityTypeID = c.Int(nullable: false),
                        BeforeReminder = c.Int(nullable: false),
                        FirstReminder = c.Int(nullable: false),
                        SecondReminder = c.Int(nullable: false),
                        ActivityType_ActivityTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ActivityType", t => t.ActivityType_ActivityTypeID, cascadeDelete: true)
                .Index(t => t.ActivityType_ActivityTypeID);
            
            CreateTable(
                "dbo.ActivityType",
                c => new
                    {
                        ActivityTypeID = c.Int(nullable: false),
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        GracePeriod = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityTypeID);
            
            CreateTable(
                "dbo.ActivityThreshold",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ActivityTypeID = c.Int(nullable: false),
                        Code = c.String(nullable: false, maxLength: 3),
                        Min = c.Int(),
                        Max = c.Int(),
                        MinOperator = c.String(maxLength: 10),
                        Operator = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ActivityStatus", t => t.Code, cascadeDelete: true)
                .ForeignKey("dbo.ActivityType", t => t.ActivityTypeID, cascadeDelete: true)
                .Index(t => t.ActivityTypeID)
                .Index(t => t.Code);
            
            CreateTable(
                "dbo.ActivityStatus",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 3),
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Color = c.String(nullable: false),
                        Order = c.Int(),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.ActivityWorkflow",
                c => new
                    {
                        ActivityWorkflowID = c.Int(nullable: false),
                        ID = c.Int(nullable: false, identity: true),
                        BaseWorkflowID = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        ActivityTypeID = c.Int(),
                        IsDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityWorkflowID)
                .ForeignKey("dbo.BaseWorkflows", t => t.BaseWorkflowID, cascadeDelete: true)
                .ForeignKey("dbo.ActivityType", t => t.ActivityTypeID, cascadeDelete: true)
                .Index(t => t.BaseWorkflowID)
                .Index(t => t.ActivityTypeID);
            
            CreateTable(
                "dbo.ActivityWorkflowSteps",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Approver = c.Int(nullable: false),
                        CommentMandatory = c.Boolean(nullable: false),
                        ActivityWorkflowID = c.Int(nullable: false),
                        IsGroup = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ActivityWorkflow", t => t.ActivityWorkflowID, cascadeDelete: true)
                .Index(t => t.ActivityWorkflowID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityWorkflow", "ActivityTypeID", "dbo.ActivityType");
            DropForeignKey("dbo.ActivityWorkflow", "BaseWorkflowID", "dbo.BaseWorkflows");
            DropForeignKey("dbo.ActivityWorkflowSteps", "ActivityWorkflowID", "dbo.ActivityWorkflow");
            DropForeignKey("dbo.ActivityThreshold", "ActivityTypeID", "dbo.ActivityType");
            DropForeignKey("dbo.ActivityThreshold", "Code", "dbo.ActivityStatus");
            DropForeignKey("dbo.ActivityReminderConfiguration", "ActivityType_ActivityTypeID", "dbo.ActivityType");
            DropIndex("dbo.ActivityWorkflowSteps", new[] { "ActivityWorkflowID" });
            DropIndex("dbo.ActivityWorkflow", new[] { "ActivityTypeID" });
            DropIndex("dbo.ActivityWorkflow", new[] { "BaseWorkflowID" });
            DropIndex("dbo.ActivityThreshold", new[] { "Code" });
            DropIndex("dbo.ActivityThreshold", new[] { "ActivityTypeID" });
            DropIndex("dbo.ActivityReminderConfiguration", new[] { "ActivityType_ActivityTypeID" });
            DropTable("dbo.ActivityWorkflowSteps");
            DropTable("dbo.ActivityWorkflow");
            DropTable("dbo.ActivityStatus");
            DropTable("dbo.ActivityThreshold");
            DropTable("dbo.ActivityType");
            DropTable("dbo.ActivityReminderConfiguration");
        }
    }
}
