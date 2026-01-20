namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_DivisionalRelation : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DivisionalObjective", new[] { "OrgStructureId" });
            AlterColumn("dbo.DivisionalObjective", "OrgStructureId", c => c.Int());
            CreateIndex("dbo.DivisionalObjective", "OrgStructureId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DivisionalObjective", new[] { "OrgStructureId" });
            AlterColumn("dbo.DivisionalObjective", "OrgStructureId", c => c.Int(nullable: false));
            CreateIndex("dbo.DivisionalObjective", "OrgStructureId");
        }
    }
}
