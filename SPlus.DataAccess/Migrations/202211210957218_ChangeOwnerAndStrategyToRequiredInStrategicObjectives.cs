namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeOwnerAndStrategyToRequiredInStrategicObjectives : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StrategicObjective", new[] { "StrategyID" });
            DropIndex("dbo.StrategicObjective", new[] { "Owner" });
            AlterColumn("dbo.StrategicObjective", "StrategyID", c => c.Int(nullable: false));
            AlterColumn("dbo.StrategicObjective", "EnglishDescription", c => c.String());
            AlterColumn("dbo.StrategicObjective", "Owner", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.StrategicObjective", "StrategyID");
            CreateIndex("dbo.StrategicObjective", "Owner");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StrategicObjective", new[] { "Owner" });
            DropIndex("dbo.StrategicObjective", new[] { "StrategyID" });
            AlterColumn("dbo.StrategicObjective", "Owner", c => c.String(maxLength: 128));
            AlterColumn("dbo.StrategicObjective", "EnglishDescription", c => c.String(nullable: false));
            AlterColumn("dbo.StrategicObjective", "StrategyID", c => c.Int());
            CreateIndex("dbo.StrategicObjective", "Owner");
            CreateIndex("dbo.StrategicObjective", "StrategyID");
        }
    }
}
