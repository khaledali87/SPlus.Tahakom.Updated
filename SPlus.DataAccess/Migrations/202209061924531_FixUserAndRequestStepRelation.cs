namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixUserAndRequestStepRelation : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.WFRequestStep", new[] { "ActionBy" });
            AlterColumn("dbo.WFRequestStep", "ActionBy", c => c.String(maxLength: 255));
            CreateIndex("dbo.WFRequestStep", "ActionBy");
        }
        
        public override void Down()
        {
            DropIndex("dbo.WFRequestStep", new[] { "ActionBy" });
            AlterColumn("dbo.WFRequestStep", "ActionBy", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.WFRequestStep", "ActionBy");
        }
    }
}
