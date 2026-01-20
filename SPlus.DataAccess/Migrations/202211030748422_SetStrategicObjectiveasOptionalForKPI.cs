namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetStrategicObjectiveasOptionalForKPI : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.KPI", new[] { "StrategicObjectiveID" });
            AlterColumn("dbo.KPI", "StrategicObjectiveID", c => c.Int());
            CreateIndex("dbo.KPI", "StrategicObjectiveID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.KPI", new[] { "StrategicObjectiveID" });
            AlterColumn("dbo.KPI", "StrategicObjectiveID", c => c.Int(nullable: false));
            CreateIndex("dbo.KPI", "StrategicObjectiveID");
        }
    }
}
