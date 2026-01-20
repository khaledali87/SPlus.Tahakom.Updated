namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivitiesAndItsRelatedTables : DbMigration
    {
        public override void Up()
        {
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
                        StartDate = c.String(nullable: false),
                        Baseline = c.DateTime(nullable: false),
                        BaselineDate = c.DateTime(nullable: false),
                        Champion = c.String(nullable: false, maxLength: 128),
                        Owner = c.String(nullable: false, maxLength: 128),
                        ActivityTypeID = c.Int(nullable: false),
                        StrategicObjectiveID = c.Int(nullable: false),
                        Years = c.Int(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        RequireUpdate = c.Boolean(nullable: false),
                        ManualUnLock = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.Champion)
                .ForeignKey("dbo.Users", t => t.Owner)
                .ForeignKey("dbo.StrategicObjective", t => t.StrategicObjectiveID, cascadeDelete: true)
                .ForeignKey("dbo.ActivityType", t => t.ActivityTypeID, cascadeDelete: true)
                .Index(t => t.Champion)
                .Index(t => t.Owner)
                .Index(t => t.ActivityTypeID)
                .Index(t => t.StrategicObjectiveID);
            
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
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Activity", t => t.ActivityID, cascadeDelete: true)
                .Index(t => t.ActivityID);
            
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
                        RangeDate = c.DateTime(),
                        AllowUpdate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Activity", t => t.ActivityID, cascadeDelete: true)
                .Index(t => t.ActivityID);
            
            AddColumn("dbo.ParameterValues", "ActivityMeasure_ID", c => c.Int());
            CreateIndex("dbo.ParameterValues", "ActivityMeasure_ID");
            AddForeignKey("dbo.ParameterValues", "ActivityMeasure_ID", "dbo.ActivityMeasures", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityMeasures", "ActivityID", "dbo.Activity");
            DropForeignKey("dbo.ParameterValues", "ActivityMeasure_ID", "dbo.ActivityMeasures");
            DropForeignKey("dbo.Activity", "ActivityTypeID", "dbo.ActivityType");
            DropForeignKey("dbo.Activity", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropForeignKey("dbo.Activity", "Owner", "dbo.Users");
            DropForeignKey("dbo.Activity", "Champion", "dbo.Users");
            DropForeignKey("dbo.ActivityComments", "ActivityID", "dbo.Activity");
            DropIndex("dbo.ParameterValues", new[] { "ActivityMeasure_ID" });
            DropIndex("dbo.ActivityMeasures", new[] { "ActivityID" });
            DropIndex("dbo.ActivityComments", new[] { "ActivityID" });
            DropIndex("dbo.Activity", new[] { "StrategicObjectiveID" });
            DropIndex("dbo.Activity", new[] { "ActivityTypeID" });
            DropIndex("dbo.Activity", new[] { "Owner" });
            DropIndex("dbo.Activity", new[] { "Champion" });
            DropColumn("dbo.ParameterValues", "ActivityMeasure_ID");
            DropTable("dbo.ActivityMeasures");
            DropTable("dbo.ActivityComments");
            DropTable("dbo.Activity");
        }
    }
}
