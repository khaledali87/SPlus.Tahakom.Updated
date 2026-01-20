namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDivisionalObjectiveRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DivisionalObjective", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropIndex("dbo.DivisionalObjective", new[] { "StrategicObjectiveID" });
            AlterColumn("dbo.DivisionalObjective", "StrategicObjectiveID", c => c.Int(nullable: false));
            CreateIndex("dbo.DivisionalObjective", "StrategicObjectiveID");
            AddForeignKey("dbo.DivisionalObjective", "StrategicObjectiveID", "dbo.StrategicObjective", "ID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DivisionalObjective", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropIndex("dbo.DivisionalObjective", new[] { "StrategicObjectiveID" });
            AlterColumn("dbo.DivisionalObjective", "StrategicObjectiveID", c => c.Int());
            CreateIndex("dbo.DivisionalObjective", "StrategicObjectiveID");
            AddForeignKey("dbo.DivisionalObjective", "StrategicObjectiveID", "dbo.StrategicObjective", "ID");
        }
    }
}
