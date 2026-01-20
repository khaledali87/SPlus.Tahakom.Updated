namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveStrategyAndStrategicObjectiveRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StrategicObjective", "StrategyID", "dbo.Strategy");
            DropIndex("dbo.StrategicObjective", new[] { "StrategyID" });
            DropColumn("dbo.StrategicObjective", "StrategyID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StrategicObjective", "StrategyID", c => c.Int(nullable: false));
            CreateIndex("dbo.StrategicObjective", "StrategyID");
            AddForeignKey("dbo.StrategicObjective", "StrategyID", "dbo.Strategy", "ID");
        }
    }
}
