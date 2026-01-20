namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixKPIUpdateformAndWFRequestRelations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests");
            DropIndex("dbo.WFRequestStep", new[] { "WFRequest_ID" });
            DropColumn("dbo.WFRequestStep", "RequestID");
            RenameColumn(table: "dbo.WFRequestStep", name: "WFRequest_ID", newName: "RequestID");
            AddColumn("dbo.WFFormUpdateKPI", "WFRequest_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.WFRequestStep", "RequestID", c => c.Int(nullable: false));
            CreateIndex("dbo.WFRequestStep", "RequestID");
            CreateIndex("dbo.WFFormUpdateKPI", "WFRequest_ID");
            AddForeignKey("dbo.WFFormUpdateKPI", "WFRequest_ID", "dbo.WFRequests", "ID", cascadeDelete: true);
            AddForeignKey("dbo.WFRequestStep", "RequestID", "dbo.WFRequests", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WFRequestStep", "RequestID", "dbo.WFRequests");
            DropForeignKey("dbo.WFFormUpdateKPI", "WFRequest_ID", "dbo.WFRequests");
            DropIndex("dbo.WFFormUpdateKPI", new[] { "WFRequest_ID" });
            DropIndex("dbo.WFRequestStep", new[] { "RequestID" });
            AlterColumn("dbo.WFRequestStep", "RequestID", c => c.Int());
            DropColumn("dbo.WFFormUpdateKPI", "WFRequest_ID");
            RenameColumn(table: "dbo.WFRequestStep", name: "RequestID", newName: "WFRequest_ID");
            AddColumn("dbo.WFRequestStep", "RequestID", c => c.Int(nullable: false));
            CreateIndex("dbo.WFRequestStep", "WFRequest_ID");
            AddForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests", "ID");
        }
    }
}
