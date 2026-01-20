namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateKPIUpdateFormAndWFRequestRelations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests");
            DropIndex("dbo.WFRequestStep", new[] { "WFRequest_ID" });
            DropColumn("dbo.WFRequestStep", "RequestID");
            RenameColumn(table: "dbo.WFRequestStep", name: "WFRequest_ID", newName: "RequestID");
            CreateTable(
                "dbo.WFFormUpdateKPI",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MeasureID = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OldValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueDate = c.DateTime(nullable: false),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.WFRequests", "WFFormUpdateKPI_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.WFRequestStep", "RequestID", c => c.Int(nullable: false));
            CreateIndex("dbo.WFRequests", "WFFormUpdateKPI_ID");
            CreateIndex("dbo.WFRequestStep", "RequestID");
            AddForeignKey("dbo.WFRequests", "WFFormUpdateKPI_ID", "dbo.WFFormUpdateKPI", "ID", cascadeDelete: true);
            AddForeignKey("dbo.WFRequestStep", "RequestID", "dbo.WFRequests", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WFRequestStep", "RequestID", "dbo.WFRequests");
            DropForeignKey("dbo.WFRequests", "WFFormUpdateKPI_ID", "dbo.WFFormUpdateKPI");
            DropIndex("dbo.WFRequestStep", new[] { "RequestID" });
            DropIndex("dbo.WFRequests", new[] { "WFFormUpdateKPI_ID" });
            AlterColumn("dbo.WFRequestStep", "RequestID", c => c.Int());
            DropColumn("dbo.WFRequests", "WFFormUpdateKPI_ID");
            DropTable("dbo.WFFormUpdateKPI");
            RenameColumn(table: "dbo.WFRequestStep", name: "RequestID", newName: "WFRequest_ID");
            AddColumn("dbo.WFRequestStep", "RequestID", c => c.Int(nullable: false));
            CreateIndex("dbo.WFRequestStep", "WFRequest_ID");
            AddForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests", "ID");
        }
    }
}
