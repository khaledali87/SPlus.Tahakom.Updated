namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateParameterAndParameterValuesTableAndRelations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ParameterValues",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MeasureID = c.Int(nullable: false),
                        ParameterID = c.Int(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.KPIMeasures", t => t.MeasureID, cascadeDelete: true)
                .Index(t => t.MeasureID);
            
            CreateTable(
                "dbo.Parameters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ParameterName = c.String(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpdateMethod = c.String(),
                        Description = c.String(nullable: false),
                        KPIID = c.Int(nullable: false),
                        FieldID = c.String(),
                        ParamId = c.String(),
                        AggregationType = c.String(),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.KPI", t => t.KPIID, cascadeDelete: true)
                .Index(t => t.KPIID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Parameters", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.ParameterValues", "MeasureID", "dbo.KPIMeasures");
            DropIndex("dbo.Parameters", new[] { "KPIID" });
            DropIndex("dbo.ParameterValues", new[] { "MeasureID" });
            DropTable("dbo.Parameters");
            DropTable("dbo.ParameterValues");
        }
    }
}
