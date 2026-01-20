namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StrategicInitiativeUpdateTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StrategicInitiative", "EnglishName");
            DropColumn("dbo.StrategicInitiative", "ArabicName");
            DropColumn("dbo.StrategicInitiative", "EnglishDescription");
            DropColumn("dbo.StrategicInitiative", "ArabicDescription");
            DropColumn("dbo.StrategicInitiative", "Manager");
            DropColumn("dbo.StrategicInitiative", "StartDate");
            DropColumn("dbo.StrategicInitiative", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StrategicInitiative", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.StrategicInitiative", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.StrategicInitiative", "Manager", c => c.String(nullable: false));
            AddColumn("dbo.StrategicInitiative", "ArabicDescription", c => c.String());
            AddColumn("dbo.StrategicInitiative", "EnglishDescription", c => c.String());
            AddColumn("dbo.StrategicInitiative", "ArabicName", c => c.String(nullable: false));
            AddColumn("dbo.StrategicInitiative", "EnglishName", c => c.String(nullable: false));
        }
    }
}
