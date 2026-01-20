namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedivision_V3 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.KPI", new[] { "StrategicObjectiveID" });
            AddColumn("dbo.KPI", "DivisionalObjectiveID", c => c.Int());
            AlterColumn("dbo.KPI", "StrategicObjectiveID", c => c.Int());
            AlterColumn("dbo.DivisionalObjective", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            CreateIndex("dbo.KPI", "StrategicObjectiveID");
            CreateIndex("dbo.KPI", "DivisionalObjectiveID");
            AddForeignKey("dbo.KPI", "DivisionalObjectiveID", "dbo.DivisionalObjective", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KPI", "DivisionalObjectiveID", "dbo.DivisionalObjective");
            DropIndex("dbo.KPI", new[] { "DivisionalObjectiveID" });
            DropIndex("dbo.KPI", new[] { "StrategicObjectiveID" });
            AlterColumn("dbo.DivisionalObjective", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KPI", "StrategicObjectiveID", c => c.Int(nullable: false));
            DropColumn("dbo.KPI", "DivisionalObjectiveID");
            CreateIndex("dbo.KPI", "StrategicObjectiveID");
        }
    }
}
