namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupRequestAndDelegationFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow");
            CreateTable(
                "dbo.WFRequests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WorkflowID = c.Int(nullable: false),
                        RequestedBy = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false, storeType: "date"),
                        Modified = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WFRequestStep",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RequestID = c.Int(nullable: false),
                        EnglishName = c.String(),
                        ArabicName = c.String(),
                        Order = c.Int(nullable: false),
                        Approver = c.String(nullable: false),
                        WorkflowStepID = c.Int(nullable: false),
                        ActionBy = c.String(),
                        Comments = c.String(),
                        IsGroup = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        WFRequest_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.WFRequests", t => t.WFRequest_ID)
                .Index(t => t.WFRequest_ID);
            
            AddForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow", "WorkflowID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow");
            DropForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests");
            DropIndex("dbo.WFRequestStep", new[] { "WFRequest_ID" });
            DropTable("dbo.WFRequestStep");
            DropTable("dbo.WFRequests");
            AddForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow", "WorkflowID", cascadeDelete: true);
        }
    }
}
