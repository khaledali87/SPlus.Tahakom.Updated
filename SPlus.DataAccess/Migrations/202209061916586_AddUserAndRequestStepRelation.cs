namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAndRequestStepRelation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WFRequestStep", "ActionBy", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.WFRequestStep", "ActionBy");
            AddForeignKey("dbo.WFRequestStep", "ActionBy", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WFRequestStep", "ActionBy", "dbo.Users");
            DropIndex("dbo.WFRequestStep", new[] { "ActionBy" });
            AlterColumn("dbo.WFRequestStep", "ActionBy", c => c.String());
        }
    }
}
