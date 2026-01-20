namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPITable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KPI", "DepartmentID", "dbo.OrgStructure");
            DropForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure");
            DropForeignKey("dbo.KPIsInitiatives", "StrategicInitiativeID", "dbo.StrategicInitiative");
            DropForeignKey("dbo.KPIsInitiatives", "KPIID", "dbo.KPI");
            DropForeignKey("dbo.KPI", "StrategyID", "dbo.Strategy");
            DropIndex("dbo.KPI", new[] { "StrategyID" });
            DropIndex("dbo.KPI", new[] { "StrategicObjectiveID" });
            DropIndex("dbo.KPI", new[] { "DivisionID" });
            DropIndex("dbo.KPI", new[] { "DepartmentID" });
            DropIndex("dbo.KPIsInitiatives", new[] { "KPIID" });
            DropIndex("dbo.KPIsInitiatives", new[] { "StrategicInitiativeID" });
            AlterColumn("dbo.KPI", "EnglishDescription", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "ArabicDescription", c => c.String());
            AlterColumn("dbo.KPI", "EnglishEquation", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "ArabicEquation", c => c.String());
            AlterColumn("dbo.KPI", "StrategicObjectiveID", c => c.Int(nullable: false));
            CreateIndex("dbo.KPI", "StrategicObjectiveID");
            DropColumn("dbo.KPI", "StrategyID");
            DropColumn("dbo.KPI", "DivisionID");
            DropColumn("dbo.KPI", "DepartmentID");
            DropTable("dbo.KPIsInitiatives");
            DropTable("dbo.StrategicInitiative");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StrategicInitiative",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProjectUID = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.KPIsInitiatives",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KPIID = c.Int(nullable: false),
                        StrategicInitiativeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.KPI", "DepartmentID", c => c.Int());
            AddColumn("dbo.KPI", "DivisionID", c => c.Int());
            AddColumn("dbo.KPI", "StrategyID", c => c.Int());
            DropIndex("dbo.KPI", new[] { "StrategicObjectiveID" });
            AlterColumn("dbo.KPI", "StrategicObjectiveID", c => c.Int());
            AlterColumn("dbo.KPI", "ArabicEquation", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "EnglishEquation", c => c.String());
            AlterColumn("dbo.KPI", "ArabicDescription", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "EnglishDescription", c => c.String());
            CreateIndex("dbo.KPIsInitiatives", "StrategicInitiativeID");
            CreateIndex("dbo.KPIsInitiatives", "KPIID");
            CreateIndex("dbo.KPI", "DepartmentID");
            CreateIndex("dbo.KPI", "DivisionID");
            CreateIndex("dbo.KPI", "StrategicObjectiveID");
            CreateIndex("dbo.KPI", "StrategyID");
            AddForeignKey("dbo.KPI", "StrategyID", "dbo.Strategy", "ID");
            AddForeignKey("dbo.KPIsInitiatives", "KPIID", "dbo.KPI", "ID", cascadeDelete: true);
            AddForeignKey("dbo.KPIsInitiatives", "StrategicInitiativeID", "dbo.StrategicInitiative", "ID", cascadeDelete: true);
            AddForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure", "ID");
            AddForeignKey("dbo.KPI", "DepartmentID", "dbo.OrgStructure", "ID");
        }
    }
}
