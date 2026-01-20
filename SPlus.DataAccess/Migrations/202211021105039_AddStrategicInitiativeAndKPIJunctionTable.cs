namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStrategicInitiativeAndKPIJunctionTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StrategicInitiative", "KPIID", "dbo.KPI");
            DropIndex("dbo.StrategicInitiative", new[] { "KPIID" });
            CreateTable(
                "dbo.KPIsInitiatives",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KPIID = c.Int(nullable: false),
                        StrategicInitiativeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StrategicInitiative", t => t.StrategicInitiativeID, cascadeDelete: true)
                .ForeignKey("dbo.KPI", t => t.KPIID, cascadeDelete: true)
                .Index(t => t.KPIID)
                .Index(t => t.StrategicInitiativeID);
            
            DropColumn("dbo.StrategicInitiative", "KPIID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StrategicInitiative", "KPIID", c => c.Int(nullable: false));
            DropForeignKey("dbo.KPIsInitiatives", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.KPIsInitiatives", "StrategicInitiativeID", "dbo.StrategicInitiative");
            DropIndex("dbo.KPIsInitiatives", new[] { "StrategicInitiativeID" });
            DropIndex("dbo.KPIsInitiatives", new[] { "KPIID" });
            DropTable("dbo.KPIsInitiatives");
            CreateIndex("dbo.StrategicInitiative", "KPIID");
            AddForeignKey("dbo.StrategicInitiative", "KPIID", "dbo.KPI", "ID", cascadeDelete: true);
        }
    }
}
