namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateWorkflowBaseTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BaseWorkflows",
                c => new
                    {
                        BaseWorkflowID = c.Int(nullable: false),
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.BaseWorkflowID);
            
            AddColumn("dbo.Workflow", "BaseWorkflowID", c => c.Int(nullable: false));
            CreateIndex("dbo.Workflow", "BaseWorkflowID");
            AddForeignKey("dbo.Workflow", "BaseWorkflowID", "dbo.BaseWorkflows", "BaseWorkflowID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Workflow", "BaseWorkflowID", "dbo.BaseWorkflows");
            DropIndex("dbo.Workflow", new[] { "BaseWorkflowID" });
            DropColumn("dbo.Workflow", "BaseWorkflowID");
            DropTable("dbo.BaseWorkflows");
        }
    }
}
