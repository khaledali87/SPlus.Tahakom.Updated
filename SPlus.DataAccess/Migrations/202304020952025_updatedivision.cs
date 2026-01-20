namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedivision : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DivisionalObjective", "OrgStructure_ID", "dbo.OrgStructure");
            DropIndex("dbo.DivisionalObjective", new[] { "OrgStructure_ID" });
            RenameColumn(table: "dbo.DivisionalObjective", name: "OrgStructure_ID", newName: "OrgStructureId");
            AlterColumn("dbo.DivisionalObjective", "OrgStructureId", c => c.Int(nullable: false));
            CreateIndex("dbo.DivisionalObjective", "OrgStructureId");
            AddForeignKey("dbo.DivisionalObjective", "OrgStructureId", "dbo.OrgStructure", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DivisionalObjective", "OrgStructureId", "dbo.OrgStructure");
            DropIndex("dbo.DivisionalObjective", new[] { "OrgStructureId" });
            AlterColumn("dbo.DivisionalObjective", "OrgStructureId", c => c.Int());
            RenameColumn(table: "dbo.DivisionalObjective", name: "OrgStructureId", newName: "OrgStructure_ID");
            CreateIndex("dbo.DivisionalObjective", "OrgStructure_ID");
            AddForeignKey("dbo.DivisionalObjective", "OrgStructure_ID", "dbo.OrgStructure", "ID");
        }
    }
}
