namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplacePerspectiveWithTheme : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StrategicObjective", "PerspectiveID", "dbo.Perspective");
            DropForeignKey("dbo.Perspective", "StrategyID", "dbo.Strategy");
            DropForeignKey("dbo.StrategicObjective", "Theme_ID", "dbo.Theme");
            DropIndex("dbo.StrategicObjective", new[] { "PerspectiveID" });
            DropIndex("dbo.StrategicObjective", new[] { "Theme_ID" });
            DropIndex("dbo.Perspective", new[] { "StrategyID" });
            RenameColumn(table: "dbo.StrategicObjective", name: "Theme_ID", newName: "ThemeID");
            AddColumn("dbo.Theme", "Order", c => c.Int(nullable: false));
            AddColumn("dbo.Theme", "StrategyID", c => c.Int(nullable: false));
            AlterColumn("dbo.StrategicObjective", "ThemeID", c => c.Int(nullable: false));
            AlterColumn("dbo.Theme", "EnglishName", c => c.String(nullable: false));
            AlterColumn("dbo.Theme", "ArabicName", c => c.String(nullable: false));
            AlterColumn("dbo.Theme", "Color", c => c.String(nullable: false));
            AlterColumn("dbo.Theme", "Modified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Theme", "Created", c => c.DateTime(nullable: false));
            CreateIndex("dbo.StrategicObjective", "ThemeID");
            CreateIndex("dbo.Theme", "StrategyID");
            AddForeignKey("dbo.Theme", "StrategyID", "dbo.Strategy", "ID", cascadeDelete: true);
            AddForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme", "ID", cascadeDelete: true);
            DropColumn("dbo.StrategicObjective", "PerspectiveID");
            DropTable("dbo.Perspective");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Perspective",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        StrategyID = c.Int(nullable: false),
                        Color = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.StrategicObjective", "PerspectiveID", c => c.Int(nullable: false));
            DropForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme");
            DropForeignKey("dbo.Theme", "StrategyID", "dbo.Strategy");
            DropIndex("dbo.Theme", new[] { "StrategyID" });
            DropIndex("dbo.StrategicObjective", new[] { "ThemeID" });
            AlterColumn("dbo.Theme", "Created", c => c.DateTime());
            AlterColumn("dbo.Theme", "Modified", c => c.DateTime());
            AlterColumn("dbo.Theme", "Color", c => c.String());
            AlterColumn("dbo.Theme", "ArabicName", c => c.String());
            AlterColumn("dbo.Theme", "EnglishName", c => c.String());
            AlterColumn("dbo.StrategicObjective", "ThemeID", c => c.Int());
            DropColumn("dbo.Theme", "StrategyID");
            DropColumn("dbo.Theme", "Order");
            RenameColumn(table: "dbo.StrategicObjective", name: "ThemeID", newName: "Theme_ID");
            CreateIndex("dbo.Perspective", "StrategyID");
            CreateIndex("dbo.StrategicObjective", "Theme_ID");
            CreateIndex("dbo.StrategicObjective", "PerspectiveID");
            AddForeignKey("dbo.StrategicObjective", "Theme_ID", "dbo.Theme", "ID");
            AddForeignKey("dbo.Perspective", "StrategyID", "dbo.Strategy", "ID", cascadeDelete: true);
            AddForeignKey("dbo.StrategicObjective", "PerspectiveID", "dbo.Perspective", "ID", cascadeDelete: true);
        }
    }
}
