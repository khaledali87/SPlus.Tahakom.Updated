namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DGDA_FirstMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Values", "StrategyID", "dbo.Strategy");
            DropIndex("dbo.Values", new[] { "StrategyID" });
            AddColumn("dbo.Strategy", "IncludeAssets", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Strategy", "EnglishVision", c => c.String(nullable: false));
            AlterColumn("dbo.Strategy", "EnglishMission", c => c.String(nullable: false));
            DropColumn("dbo.Strategy", "StrategyType");
            DropTable("dbo.Values");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Values",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        StrategyID = c.Int(nullable: false),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Strategy", "StrategyType", c => c.Int(nullable: false));
            AlterColumn("dbo.Strategy", "EnglishMission", c => c.String());
            AlterColumn("dbo.Strategy", "EnglishVision", c => c.String());
            DropColumn("dbo.Strategy", "IncludeAssets");
            CreateIndex("dbo.Values", "StrategyID");
            AddForeignKey("dbo.Values", "StrategyID", "dbo.Strategy", "ID", cascadeDelete: true);
        }
    }
}
