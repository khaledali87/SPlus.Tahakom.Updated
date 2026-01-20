namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBaseWorkflowIDToWFRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WFRequests", "BaseWorkflowID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WFRequests", "BaseWorkflowID");
        }
    }
}
