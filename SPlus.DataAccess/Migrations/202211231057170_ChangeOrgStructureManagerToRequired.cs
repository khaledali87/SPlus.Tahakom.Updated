namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeOrgStructureManagerToRequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.OrgStructure", new[] { "Manager" });
            AlterColumn("dbo.OrgStructure", "Manager", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.OrgStructure", "Manager");
        }
        
        public override void Down()
        {
            DropIndex("dbo.OrgStructure", new[] { "Manager" });
            AlterColumn("dbo.OrgStructure", "Manager", c => c.String(maxLength: 128));
            CreateIndex("dbo.OrgStructure", "Manager");
        }
    }
}
