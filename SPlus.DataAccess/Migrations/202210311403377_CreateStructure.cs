namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStructure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme");
            RenameColumn(table: "dbo.StrategicObjective", name: "ThemeID", newName: "Theme_ID");
            RenameIndex(table: "dbo.StrategicObjective", name: "IX_ThemeID", newName: "IX_Theme_ID");
            CreateTable(
                "dbo.Strategy",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false, maxLength: 255),
                        ArabicName = c.String(nullable: false, maxLength: 255),
                        StrategyType = c.String(nullable: false, maxLength: 255),
                        EnglishVision = c.String(maxLength: 255),
                        ArabicVision = c.String(maxLength: 255),
                        EnglishMission = c.String(maxLength: 255),
                        ArabicMission = c.String(maxLength: 255),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.StrategicObjective", "ArabicDescription", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.StrategicObjective", "EnglishDescription", c => c.String(maxLength: 255));
            AddColumn("dbo.StrategicObjective", "Order", c => c.Int(nullable: false));
            AddColumn("dbo.StrategicObjective", "Code", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.Perspective", "Order", c => c.Int(nullable: false));
            AddColumn("dbo.Perspective", "Strategy_ID", c => c.Int());
            AlterColumn("dbo.Perspective", "Color", c => c.String(maxLength: 255));
            CreateIndex("dbo.Perspective", "Strategy_ID");
            AddForeignKey("dbo.Perspective", "Strategy_ID", "dbo.Strategy", "ID");
            AddForeignKey("dbo.StrategicObjective", "Theme_ID", "dbo.Theme", "ID");
            DropColumn("dbo.Perspective", "Icon");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Perspective", "Icon", c => c.String(maxLength: 255));
            DropForeignKey("dbo.StrategicObjective", "Theme_ID", "dbo.Theme");
            DropForeignKey("dbo.Perspective", "Strategy_ID", "dbo.Strategy");
            DropIndex("dbo.Perspective", new[] { "Strategy_ID" });
            AlterColumn("dbo.Perspective", "Color", c => c.String());
            DropColumn("dbo.Perspective", "Strategy_ID");
            DropColumn("dbo.Perspective", "Order");
            DropColumn("dbo.StrategicObjective", "Code");
            DropColumn("dbo.StrategicObjective", "Order");
            DropColumn("dbo.StrategicObjective", "EnglishDescription");
            DropColumn("dbo.StrategicObjective", "ArabicDescription");
            DropTable("dbo.Strategy");
            RenameIndex(table: "dbo.StrategicObjective", name: "IX_Theme_ID", newName: "IX_ThemeID");
            RenameColumn(table: "dbo.StrategicObjective", name: "Theme_ID", newName: "ThemeID");
            AddForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme", "ID", cascadeDelete: true);
        }
    }
}
