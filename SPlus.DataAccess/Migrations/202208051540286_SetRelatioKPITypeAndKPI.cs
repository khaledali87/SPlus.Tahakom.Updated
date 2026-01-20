namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetRelatioKPITypeAndKPI : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests");
            //DropIndex("dbo.WFRequestStep", new[] { "WFRequest_ID" });
            AddColumn("dbo.KPI", "KPITypeID", c => c.Int(nullable: false));
            CreateIndex("dbo.KPI", "KPITypeID");
            AddForeignKey("dbo.KPI", "KPITypeID", "dbo.KPIType", "KPITypeID");
            DropColumn("dbo.KPI", "KPIType");
            //DropColumn("dbo.WFRequestStep", "WFRequest_ID");
            //DropTable("dbo.WFRequests");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.WFRequests",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Title = c.String(),
            //            WorkflowID = c.Int(nullable: false),
            //            RelatedItemID = c.Int(nullable: false),
            //            RequestedBy = c.String(nullable: false),
            //            Status = c.Int(nullable: false),
            //            Created = c.DateTime(nullable: false, storeType: "date"),
            //            Modified = c.DateTime(nullable: false, storeType: "date"),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //AddColumn("dbo.WFRequestStep", "WFRequest_ID", c => c.Int());
            AddColumn("dbo.KPI", "KPIType", c => c.String(maxLength: 255));
            DropForeignKey("dbo.KPI", "KPITypeID", "dbo.KPIType");
            DropIndex("dbo.KPI", new[] { "KPITypeID" });
            DropColumn("dbo.KPI", "KPITypeID");
            //CreateIndex("dbo.WFRequestStep", "WFRequest_ID");
            //AddForeignKey("dbo.WFRequestStep", "WFRequest_ID", "dbo.WFRequests", "ID");
        }
    }
}
