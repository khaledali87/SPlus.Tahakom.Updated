namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CleanSolution : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActivityComments", "ActivityID", "dbo.Activity");
            DropForeignKey("dbo.ActivityMeasures", "ActivityID", "dbo.Activity");
            DropForeignKey("dbo.Activity", "ActivityTypeID", "dbo.ActivityType");
            DropForeignKey("dbo.ActivityReminderConfiguration", "ActivityType_ActivityTypeID", "dbo.ActivityType");
            DropForeignKey("dbo.ActivityThreshold", "Code", "dbo.Status");
            DropForeignKey("dbo.Activity", "Champion", "dbo.Users");
            DropForeignKey("dbo.Activity", "DivisionalObjectiveID", "dbo.DivisionalObjective");
            DropForeignKey("dbo.Assets", "DistrictID", "dbo.Districts");
            DropForeignKey("dbo.Districts", "WorkstreamID", "dbo.Workstreams");
            DropForeignKey("dbo.Assets", "Manager", "dbo.Users");
            DropForeignKey("dbo.Activity", "Owner", "dbo.Users");
            DropForeignKey("dbo.Activity", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropForeignKey("dbo.ActivityWorkflowSteps", "ActivityWorkflowID", "dbo.ActivityWorkflow");
            DropForeignKey("dbo.ActivityWorkflow", "BaseWorkflowID", "dbo.BaseWorkflows");
            DropForeignKey("dbo.ActivityThreshold", "ActivityTypeID", "dbo.ActivityType");
            DropForeignKey("dbo.ActivityWorkflow", "ActivityTypeID", "dbo.ActivityType");
            DropIndex("dbo.Activity", new[] { "Champion" });
            DropIndex("dbo.Activity", new[] { "Owner" });
            DropIndex("dbo.Activity", new[] { "ActivityTypeID" });
            DropIndex("dbo.Activity", new[] { "StrategicObjectiveID" });
            DropIndex("dbo.Activity", new[] { "DivisionalObjectiveID" });
            DropIndex("dbo.ActivityComments", new[] { "ActivityID" });
            DropIndex("dbo.ActivityMeasures", new[] { "ActivityID" });
            DropIndex("dbo.ActivityReminderConfiguration", new[] { "ActivityType_ActivityTypeID" });
            DropIndex("dbo.ActivityThreshold", new[] { "ActivityTypeID" });
            DropIndex("dbo.ActivityThreshold", new[] { "Code" });
            DropIndex("dbo.Assets", new[] { "Manager" });
            DropIndex("dbo.Assets", new[] { "DistrictID" });
            DropIndex("dbo.Districts", new[] { "WorkstreamID" });
            DropIndex("dbo.ActivityWorkflow", new[] { "BaseWorkflowID" });
            DropIndex("dbo.ActivityWorkflow", new[] { "ActivityTypeID" });
            DropIndex("dbo.ActivityWorkflowSteps", new[] { "ActivityWorkflowID" });
            AddColumn("dbo.Theme", "BackgroundColor", c => c.String(nullable: false));
            AddColumn("dbo.Theme", "FrameColor", c => c.String(nullable: false));
            AddColumn("dbo.Strategy", "IsSloganEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Strategy", "EnglishSlogan", c => c.String(nullable: false));
            AddColumn("dbo.Strategy", "ArabicSlogan", c => c.String(nullable: false));
            AddColumn("dbo.Strategy", "IsVisionEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Strategy", "IsMissionEnabled", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Strategy", "ArabicVision", c => c.String(nullable: false));
            AlterColumn("dbo.Strategy", "ArabicMission", c => c.String(nullable: false));
            DropColumn("dbo.Theme", "Color");
            DropColumn("dbo.Strategy", "IncludeAssets");
            DropTable("dbo.Activity");
            DropTable("dbo.ActivityComments");
            DropTable("dbo.ActivityMeasures");
            DropTable("dbo.ActivityType");
            DropTable("dbo.ActivityReminderConfiguration");
            DropTable("dbo.ActivityThreshold");
            DropTable("dbo.Assets");
            DropTable("dbo.Districts");
            DropTable("dbo.Workstreams");
            DropTable("dbo.ActivityWorkflow");
            DropTable("dbo.ActivityWorkflowSteps");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ActivityWorkflowID);
            
            CreateTable(
                "dbo.Workstreams",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArabicName = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArabicName = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        WorkstreamID = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArabicName = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        EnglishDescription = c.String(nullable: false),
                        ArabicDescription = c.String(),
                        Manager = c.String(nullable: false, maxLength: 128),
                        StartDate = c.DateTime(nullable: false),
                        Progress = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        EndDate = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        DistrictID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
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
                "dbo.ActivityMeasures",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Target = c.DateTime(nullable: false),
                        ForecastDate = c.DateTime(),
                        DueDate = c.DateTime(nullable: false),
                        Status = c.String(nullable: false),
                        UpdateDate = c.DateTime(),
                        ActivityID = c.Int(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Range = c.Decimal(precision: 18, scale: 2),
                        AllowUpdate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ActivityComments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ActivityID = c.Int(nullable: false),
                        Comment = c.String(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Activity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArabicName = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        EnglishDescription = c.String(nullable: false),
                        ArabicDescription = c.String(),
                        Frequency = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Champion = c.String(nullable: false, maxLength: 128),
                        Owner = c.String(nullable: false, maxLength: 128),
                        ActivityTypeID = c.Int(nullable: false),
                        StrategicObjectiveID = c.Int(),
                        DivisionalObjectiveID = c.Int(),
                        Years = c.Int(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        RequireUpdate = c.Boolean(nullable: false),
                        ManualUnLock = c.Boolean(nullable: false),
                        IsVerified = c.Boolean(),
                        IsCompleted = c.Boolean(),
                        UnlockDate = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Strategy", "IncludeAssets", c => c.Boolean(nullable: false));
            AddColumn("dbo.Theme", "Color", c => c.String(nullable: false));
            AlterColumn("dbo.Strategy", "ArabicMission", c => c.String());
            AlterColumn("dbo.Strategy", "ArabicVision", c => c.String());
            DropColumn("dbo.Strategy", "IsMissionEnabled");
            DropColumn("dbo.Strategy", "IsVisionEnabled");
            DropColumn("dbo.Strategy", "ArabicSlogan");
            DropColumn("dbo.Strategy", "EnglishSlogan");
            DropColumn("dbo.Strategy", "IsSloganEnabled");
            DropColumn("dbo.Theme", "FrameColor");
            DropColumn("dbo.Theme", "BackgroundColor");
            CreateIndex("dbo.ActivityWorkflowSteps", "ActivityWorkflowID");
            CreateIndex("dbo.ActivityWorkflow", "ActivityTypeID");
            CreateIndex("dbo.ActivityWorkflow", "BaseWorkflowID");
            CreateIndex("dbo.Districts", "WorkstreamID");
            CreateIndex("dbo.Assets", "DistrictID");
            CreateIndex("dbo.Assets", "Manager");
            CreateIndex("dbo.ActivityThreshold", "Code");
            CreateIndex("dbo.ActivityThreshold", "ActivityTypeID");
            CreateIndex("dbo.ActivityReminderConfiguration", "ActivityType_ActivityTypeID");
            CreateIndex("dbo.ActivityMeasures", "ActivityID");
            CreateIndex("dbo.ActivityComments", "ActivityID");
            CreateIndex("dbo.Activity", "DivisionalObjectiveID");
            CreateIndex("dbo.Activity", "StrategicObjectiveID");
            CreateIndex("dbo.Activity", "ActivityTypeID");
            CreateIndex("dbo.Activity", "Owner");
            CreateIndex("dbo.Activity", "Champion");
            AddForeignKey("dbo.ActivityWorkflow", "ActivityTypeID", "dbo.ActivityType", "ActivityTypeID", cascadeDelete: true);
            AddForeignKey("dbo.ActivityThreshold", "ActivityTypeID", "dbo.ActivityType", "ActivityTypeID", cascadeDelete: true);
            AddForeignKey("dbo.ActivityWorkflow", "BaseWorkflowID", "dbo.BaseWorkflows", "BaseWorkflowID", cascadeDelete: true);
            AddForeignKey("dbo.ActivityWorkflowSteps", "ActivityWorkflowID", "dbo.ActivityWorkflow", "ActivityWorkflowID", cascadeDelete: true);
            AddForeignKey("dbo.Activity", "StrategicObjectiveID", "dbo.StrategicObjective", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Activity", "Owner", "dbo.Users", "UserName");
            AddForeignKey("dbo.Assets", "Manager", "dbo.Users", "UserName");
            AddForeignKey("dbo.Districts", "WorkstreamID", "dbo.Workstreams", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Assets", "DistrictID", "dbo.Districts", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Activity", "DivisionalObjectiveID", "dbo.DivisionalObjective", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Activity", "Champion", "dbo.Users", "UserName");
            AddForeignKey("dbo.ActivityThreshold", "Code", "dbo.Status", "Code", cascadeDelete: true);
            AddForeignKey("dbo.ActivityReminderConfiguration", "ActivityType_ActivityTypeID", "dbo.ActivityType", "ActivityTypeID", cascadeDelete: true);
            AddForeignKey("dbo.Activity", "ActivityTypeID", "dbo.ActivityType", "ActivityTypeID", cascadeDelete: true);
            AddForeignKey("dbo.ActivityMeasures", "ActivityID", "dbo.Activity", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ActivityComments", "ActivityID", "dbo.Activity", "ID", cascadeDelete: true);
        }
    }
}
