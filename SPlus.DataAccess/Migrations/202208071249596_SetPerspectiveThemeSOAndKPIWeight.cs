namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetPerspectiveThemeSOAndKPIWeight : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AddColumn("dbo.StrategicObjective", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AddColumn("dbo.Perspective", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AddColumn("dbo.Theme", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Theme", "Weight");
            DropColumn("dbo.Perspective", "Weight");
            DropColumn("dbo.StrategicObjective", "Weight");
            DropColumn("dbo.KPI", "Weight");
        }
    }
}
