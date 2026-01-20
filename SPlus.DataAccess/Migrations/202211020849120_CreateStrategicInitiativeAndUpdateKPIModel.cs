namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStrategicInitiativeAndUpdateKPIModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KPI", "Sponsor", "dbo.Users");
            DropIndex("dbo.KPI", new[] { "Sponsor" });
            CreateTable(
                "dbo.StrategicInitiative",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProjectUID = c.Guid(nullable: false),
                        KPIID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.KPI", t => t.KPIID, cascadeDelete: true)
                .Index(t => t.KPIID);
            
            AddColumn("dbo.KPI", "StrategyID", c => c.Int());
            CreateIndex("dbo.KPI", "StrategyID");
            AddForeignKey("dbo.KPI", "StrategyID", "dbo.Strategy", "ID");
            DropColumn("dbo.KPI", "ReferenceNo");
            DropColumn("dbo.KPI", "Sponsor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPI", "Sponsor", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.KPI", "ReferenceNo", c => c.String(nullable: false, maxLength: 255));
            DropForeignKey("dbo.KPI", "StrategyID", "dbo.Strategy");
            DropForeignKey("dbo.StrategicInitiative", "KPIID", "dbo.KPI");
            DropIndex("dbo.StrategicInitiative", new[] { "KPIID" });
            DropIndex("dbo.KPI", new[] { "StrategyID" });
            DropColumn("dbo.KPI", "StrategyID");
            DropTable("dbo.StrategicInitiative");
            CreateIndex("dbo.KPI", "Sponsor");
            AddForeignKey("dbo.KPI", "Sponsor", "dbo.Users", "UserName");
        }
    }
}
