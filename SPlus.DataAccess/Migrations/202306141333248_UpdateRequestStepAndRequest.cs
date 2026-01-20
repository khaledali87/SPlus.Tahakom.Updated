namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRequestStepAndRequest : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RequestStep", "CommentMandatory");
            DropColumn("dbo.WorkflowSteps", "CommentMandatory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkflowSteps", "CommentMandatory", c => c.Boolean(nullable: false));
            AddColumn("dbo.RequestStep", "CommentMandatory", c => c.Boolean(nullable: false));
        }
    }
}
