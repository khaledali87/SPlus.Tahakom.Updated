namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStrategicObjective : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.StrategicObjective", name: "Owner", newName: "User_UserName");
            RenameIndex(table: "dbo.StrategicObjective", name: "IX_Owner", newName: "IX_User_UserName");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.StrategicObjective", name: "IX_User_UserName", newName: "IX_Owner");
            RenameColumn(table: "dbo.StrategicObjective", name: "User_UserName", newName: "Owner");
        }
    }
}
