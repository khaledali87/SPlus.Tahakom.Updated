namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetRequiredRelationBetweenThemeAndStrategicObjective : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StrategicObjective", new[] { "ThemeID" });
            AlterColumn("dbo.StrategicObjective", "ThemeID", c => c.Int(nullable: false));
            CreateIndex("dbo.StrategicObjective", "ThemeID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StrategicObjective", new[] { "ThemeID" });
            AlterColumn("dbo.StrategicObjective", "ThemeID", c => c.Int());
            CreateIndex("dbo.StrategicObjective", "ThemeID");
        }
    }
}
