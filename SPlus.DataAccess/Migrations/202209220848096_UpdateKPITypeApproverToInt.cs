namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPITypeApproverToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkflowSteps", "Approver", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkflowSteps", "Approver", c => c.String(nullable: false));
        }
    }
}
