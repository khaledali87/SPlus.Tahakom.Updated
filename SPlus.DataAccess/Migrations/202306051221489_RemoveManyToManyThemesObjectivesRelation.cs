namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveManyToManyThemesObjectivesRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ThemesObjectives", "ThemeID", "dbo.Theme");
            DropForeignKey("dbo.ThemesObjectives", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropIndex("dbo.ThemesObjectives", new[] { "ThemeID" });
            DropIndex("dbo.ThemesObjectives", new[] { "StrategicObjectiveID" });
            AddColumn("dbo.StrategicObjective", "ThemeID", c => c.Int());
            CreateIndex("dbo.StrategicObjective", "ThemeID");
            AddForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme", "ID", cascadeDelete: true);
            DropTable("dbo.ThemesObjectives");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ThemesObjectives",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ThemeID = c.Int(nullable: false),
                        StrategicObjectiveID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme");
            DropIndex("dbo.StrategicObjective", new[] { "ThemeID" });
            DropColumn("dbo.StrategicObjective", "ThemeID");
            CreateIndex("dbo.ThemesObjectives", "StrategicObjectiveID");
            CreateIndex("dbo.ThemesObjectives", "ThemeID");
            AddForeignKey("dbo.ThemesObjectives", "StrategicObjectiveID", "dbo.StrategicObjective", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ThemesObjectives", "ThemeID", "dbo.Theme", "ID", cascadeDelete: true);
        }
    }
}
