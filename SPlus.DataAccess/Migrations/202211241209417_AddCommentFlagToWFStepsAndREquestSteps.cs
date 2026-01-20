namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentFlagToWFStepsAndREquestSteps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkflowSteps", "CommentMandatory", c => c.Boolean(nullable: false));
            AddColumn("dbo.RequestStep", "CommentMandatory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequestStep", "CommentMandatory");
            DropColumn("dbo.WorkflowSteps", "CommentMandatory");
        }
    }
}
