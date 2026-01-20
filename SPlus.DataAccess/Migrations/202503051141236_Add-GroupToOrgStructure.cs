namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupToOrgStructure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrgStructure", "GroupID", c => c.Int());
            CreateIndex("dbo.OrgStructure", "GroupID");
            AddForeignKey("dbo.OrgStructure", "GroupID", "dbo.Groups", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrgStructure", "GroupID", "dbo.Groups");
            DropIndex("dbo.OrgStructure", new[] { "GroupID" });
            DropColumn("dbo.OrgStructure", "GroupID");
        }
    }
}
