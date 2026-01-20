namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteWFRequestRelations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WFRequests", "WFFormUpdateKPI_ID", "dbo.WFFormUpdateKPI");
            DropForeignKey("dbo.WFRequestStep", "RequestID", "dbo.WFRequests");
            DropIndex("dbo.WFRequestStep", new[] { "RequestID" });
            DropIndex("dbo.WFRequests", new[] { "WFFormUpdateKPI_ID" });
            AddColumn("dbo.WFRequestStep", "WFRequest_ID", c => c.Int());
            CreateIndex("dbo.WFRequestStep", "WFRequest_ID");
            AddForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests", "ID");
            DropColumn("dbo.WFRequests", "WFFormUpdateKPI_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WFRequests", "WFFormUpdateKPI_ID", c => c.Int(nullable: false));
            DropForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests");
            DropIndex("dbo.WFRequestStep", new[] { "WFRequest_ID" });
            DropColumn("dbo.WFRequestStep", "WFRequest_ID");
            CreateIndex("dbo.WFRequests", "WFFormUpdateKPI_ID");
            CreateIndex("dbo.WFRequestStep", "RequestID");
            AddForeignKey("dbo.WFRequestStep", "RequestID", "dbo.WFRequests", "ID", cascadeDelete: true);
            AddForeignKey("dbo.WFRequests", "WFFormUpdateKPI_ID", "dbo.WFFormUpdateKPI", "ID", cascadeDelete: true);
        }
    }
}
