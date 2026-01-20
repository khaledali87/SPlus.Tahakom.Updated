namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetRelationKPITypeAndWorkflows_Fix3 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Workflow", new[] { "KPITypeID" });
            AlterColumn("dbo.Workflow", "KPITypeID", c => c.Int());
            CreateIndex("dbo.Workflow", "KPITypeID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Workflow", new[] { "KPITypeID" });
            AlterColumn("dbo.Workflow", "KPITypeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Workflow", "KPITypeID");
        }
    }
}
