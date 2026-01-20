namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStrategicObjectiveOwnerAndStrategyRelations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StrategicObjective", "StrategyID", c => c.Int());
            AddColumn("dbo.StrategicObjective", "Owner", c => c.String(maxLength: 128));
            AlterColumn("dbo.StrategicObjective", "ArabicDescription", c => c.String());
            AlterColumn("dbo.StrategicObjective", "EnglishDescription", c => c.String(nullable: false));
            CreateIndex("dbo.StrategicObjective", "StrategyID");
            CreateIndex("dbo.StrategicObjective", "Owner");
            AddForeignKey("dbo.StrategicObjective", "StrategyID", "dbo.Strategy", "ID");
            AddForeignKey("dbo.StrategicObjective", "Owner", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StrategicObjective", "Owner", "dbo.Users");
            DropForeignKey("dbo.StrategicObjective", "StrategyID", "dbo.Strategy");
            DropIndex("dbo.StrategicObjective", new[] { "Owner" });
            DropIndex("dbo.StrategicObjective", new[] { "StrategyID" });
            AlterColumn("dbo.StrategicObjective", "EnglishDescription", c => c.String());
            AlterColumn("dbo.StrategicObjective", "ArabicDescription", c => c.String(nullable: false));
            DropColumn("dbo.StrategicObjective", "Owner");
            DropColumn("dbo.StrategicObjective", "StrategyID");
        }
    }
}
