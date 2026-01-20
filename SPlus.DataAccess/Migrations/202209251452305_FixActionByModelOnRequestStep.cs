namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixActionByModelOnRequestStep : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RequestStep", new[] { "ActionByModel_UserName" });
            DropColumn("dbo.RequestStep", "ActionBy");
            RenameColumn(table: "dbo.RequestStep", name: "ActionByModel_UserName", newName: "ActionBy");
            AlterColumn("dbo.RequestStep", "ActionBy", c => c.String(maxLength: 255));
            CreateIndex("dbo.RequestStep", "ActionBy");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RequestStep", new[] { "ActionBy" });
            AlterColumn("dbo.RequestStep", "ActionBy", c => c.String());
            RenameColumn(table: "dbo.RequestStep", name: "ActionBy", newName: "ActionByModel_UserName");
            AddColumn("dbo.RequestStep", "ActionBy", c => c.String());
            CreateIndex("dbo.RequestStep", "ActionByModel_UserName");
        }
    }
}
