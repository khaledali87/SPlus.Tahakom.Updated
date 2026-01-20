namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixRelationsOfStrategicObjectiveWithThemesAndPerspectives : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StrategicObjective", "PerspectiveID", "dbo.Perspective");
            DropIndex("dbo.StrategicObjective", new[] { "PerspectiveID" });
            DropIndex("dbo.StrategicObjective", new[] { "ThemeID" });
            AddColumn("dbo.Perspective", "Color", c => c.String());
            AddColumn("dbo.Theme", "Color", c => c.String());
            AlterColumn("dbo.StrategicObjective", "PerspectiveID", c => c.Int(nullable: false));
            AlterColumn("dbo.StrategicObjective", "ThemeID", c => c.Int());
            CreateIndex("dbo.StrategicObjective", "PerspectiveID");
            CreateIndex("dbo.StrategicObjective", "ThemeID");
            AddForeignKey("dbo.StrategicObjective", "PerspectiveID", "dbo.Perspective", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StrategicObjective", "PerspectiveID", "dbo.Perspective");
            DropIndex("dbo.StrategicObjective", new[] { "ThemeID" });
            DropIndex("dbo.StrategicObjective", new[] { "PerspectiveID" });
            AlterColumn("dbo.StrategicObjective", "ThemeID", c => c.Int(nullable: false));
            AlterColumn("dbo.StrategicObjective", "PerspectiveID", c => c.Int());
            DropColumn("dbo.Theme", "Color");
            DropColumn("dbo.Perspective", "Color");
            CreateIndex("dbo.StrategicObjective", "ThemeID");
            CreateIndex("dbo.StrategicObjective", "PerspectiveID");
            AddForeignKey("dbo.StrategicObjective", "PerspectiveID", "dbo.Perspective", "ID");
        }
    }
}
