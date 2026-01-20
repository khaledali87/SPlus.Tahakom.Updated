namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixAssetsUserRelations : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Assets", new[] { "ManagerModel_UserName" });
            DropColumn("dbo.Assets", "Manager");
            RenameColumn(table: "dbo.Assets", name: "ManagerModel_UserName", newName: "Manager");
            AlterColumn("dbo.Assets", "Manager", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Assets", "Manager", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Assets", "Manager");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Assets", new[] { "Manager" });
            AlterColumn("dbo.Assets", "Manager", c => c.String(maxLength: 128));
            AlterColumn("dbo.Assets", "Manager", c => c.String(nullable: false));
            RenameColumn(table: "dbo.Assets", name: "Manager", newName: "ManagerModel_UserName");
            AddColumn("dbo.Assets", "Manager", c => c.String(nullable: false));
            CreateIndex("dbo.Assets", "ManagerModel_UserName");
        }
    }
}
