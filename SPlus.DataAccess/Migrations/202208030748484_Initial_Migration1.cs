namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Workflows", newName: "Workflow");
            RenameTable(name: "dbo.WFRequestSteps", newName: "WFRequestStep");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.WFRequestStep", newName: "WFRequestSteps");
            RenameTable(name: "dbo.Workflow", newName: "Workflows");
        }
    }
}
