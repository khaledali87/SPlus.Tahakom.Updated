namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateWorkflowIDAtWorkflowTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow");
            DropPrimaryKey("dbo.Workflow");
            AddColumn("dbo.Workflow", "WorkflowID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Workflow", "WorkflowID");
            AddForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow", "WorkflowID");
            //DropTable("dbo.WFRequestStep");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.WFRequestStep",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            RequestID = c.Int(nullable: false),
            //            Approver = c.String(nullable: false),
            //            WorkflowStepID = c.Int(nullable: false),
            //            ActionBy = c.String(),
            //            Comments = c.String(),
            //            IsGroup = c.Boolean(nullable: false),
            //            Status = c.Int(nullable: false),
            //            Created = c.DateTime(nullable: false),
            //            Modified = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            DropForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow");
            DropPrimaryKey("dbo.Workflow");
            DropColumn("dbo.Workflow", "WorkflowID");
            AddPrimaryKey("dbo.Workflow", "ID");
            AddForeignKey("dbo.WorkflowSteps", "WorkflowID", "dbo.Workflow", "ID");
        }
    }
}
