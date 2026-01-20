namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDivisionalObjective : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DivisionalObjective", name: "Owner", newName: "User_UserName");
            RenameIndex(table: "dbo.DivisionalObjective", name: "IX_Owner", newName: "IX_User_UserName");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.DivisionalObjective", name: "IX_User_UserName", newName: "IX_Owner");
            RenameColumn(table: "dbo.DivisionalObjective", name: "User_UserName", newName: "Owner");
        }
    }
}
