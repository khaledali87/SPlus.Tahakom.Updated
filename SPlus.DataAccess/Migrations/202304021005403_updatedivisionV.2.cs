namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedivisionV2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DivisionalObjective", new[] { "OwnerModel_UserName" });
            DropColumn("dbo.DivisionalObjective", "Owner");
            RenameColumn(table: "dbo.DivisionalObjective", name: "OwnerModel_UserName", newName: "Owner");
            AlterColumn("dbo.DivisionalObjective", "Owner", c => c.String(maxLength: 128));
            CreateIndex("dbo.DivisionalObjective", "Owner");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DivisionalObjective", new[] { "Owner" });
            AlterColumn("dbo.DivisionalObjective", "Owner", c => c.String());
            RenameColumn(table: "dbo.DivisionalObjective", name: "Owner", newName: "OwnerModel_UserName");
            AddColumn("dbo.DivisionalObjective", "Owner", c => c.String());
            CreateIndex("dbo.DivisionalObjective", "OwnerModel_UserName");
        }
    }
}
