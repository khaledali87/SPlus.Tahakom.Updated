namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StrategicInitiativeRemoveUserRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StrategicInitiative", "ManagerModel_UserName", "dbo.Users");
            DropIndex("dbo.StrategicInitiative", new[] { "ManagerModel_UserName" });
            DropColumn("dbo.StrategicInitiative", "ManagerModel_UserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StrategicInitiative", "ManagerModel_UserName", c => c.String(maxLength: 255));
            CreateIndex("dbo.StrategicInitiative", "ManagerModel_UserName");
            AddForeignKey("dbo.StrategicInitiative", "ManagerModel_UserName", "dbo.Users", "UserName");
        }
    }
}
