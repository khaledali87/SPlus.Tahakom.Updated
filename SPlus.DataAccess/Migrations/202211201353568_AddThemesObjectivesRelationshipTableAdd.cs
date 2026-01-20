namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddThemesObjectivesRelationshipTableAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThemesObjectives",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ThemeID = c.Int(nullable: false),
                        StrategicObjectiveID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Theme", t => t.ThemeID, cascadeDelete: true)
                .ForeignKey("dbo.StrategicObjective", t => t.StrategicObjectiveID, cascadeDelete: true)
                .Index(t => t.ThemeID)
                .Index(t => t.StrategicObjectiveID);
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.StrategicObjective", "ThemeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.ThemesObjectives", "StrategicObjectiveID", "dbo.StrategicObjective");
            DropForeignKey("dbo.ThemesObjectives", "ThemeID", "dbo.Theme");
            DropIndex("dbo.ThemesObjectives", new[] { "StrategicObjectiveID" });
            DropIndex("dbo.ThemesObjectives", new[] { "ThemeID" });
            DropTable("dbo.ThemesObjectives");
            CreateIndex("dbo.StrategicObjective", "ThemeID");
            AddForeignKey("dbo.StrategicObjective", "ThemeID", "dbo.Theme", "ID", cascadeDelete: true);
        }
    }
}
