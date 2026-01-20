namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStrategicInitiativeTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StrategicInitiative", "EnglishName", c => c.String(nullable: false));
            AddColumn("dbo.StrategicInitiative", "ArabicName", c => c.String(nullable: false));
            AddColumn("dbo.StrategicInitiative", "EnglishDescription", c => c.String());
            AddColumn("dbo.StrategicInitiative", "ArabicDescription", c => c.String());
            AddColumn("dbo.StrategicInitiative", "Manager", c => c.String(nullable: false));
            AddColumn("dbo.StrategicInitiative", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.StrategicInitiative", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.StrategicInitiative", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.StrategicInitiative", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.StrategicInitiative", "ManagerModel_UserName", c => c.String(maxLength: 255));
            CreateIndex("dbo.StrategicInitiative", "ManagerModel_UserName");
            AddForeignKey("dbo.StrategicInitiative", "ManagerModel_UserName", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StrategicInitiative", "ManagerModel_UserName", "dbo.Users");
            DropIndex("dbo.StrategicInitiative", new[] { "ManagerModel_UserName" });
            DropColumn("dbo.StrategicInitiative", "ManagerModel_UserName");
            DropColumn("dbo.StrategicInitiative", "Modified");
            DropColumn("dbo.StrategicInitiative", "Created");
            DropColumn("dbo.StrategicInitiative", "EndDate");
            DropColumn("dbo.StrategicInitiative", "StartDate");
            DropColumn("dbo.StrategicInitiative", "Manager");
            DropColumn("dbo.StrategicInitiative", "ArabicDescription");
            DropColumn("dbo.StrategicInitiative", "EnglishDescription");
            DropColumn("dbo.StrategicInitiative", "ArabicName");
            DropColumn("dbo.StrategicInitiative", "EnglishName");
        }
    }
}
