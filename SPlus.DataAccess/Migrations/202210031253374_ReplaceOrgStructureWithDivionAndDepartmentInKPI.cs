namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReplaceOrgStructureWithDivionAndDepartmentInKPI : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KPI", "OrgStructureID", "dbo.OrgStructure");
            DropIndex("dbo.KPI", new[] { "OrgStructureID" });
            RenameColumn(table: "dbo.KPI", name: "OrgStructureID", newName: "DepartmentID");
            AddColumn("dbo.KPI", "DivisionID", c => c.Int(nullable: false));
            AlterColumn("dbo.KPI", "DepartmentID", c => c.Int());
            CreateIndex("dbo.KPI", "DivisionID");
            CreateIndex("dbo.KPI", "DepartmentID");
            AddForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure", "ID", cascadeDelete: true);
            AddForeignKey("dbo.KPI", "DepartmentID", "dbo.OrgStructure", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KPI", "DepartmentID", "dbo.OrgStructure");
            DropForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure");
            DropIndex("dbo.KPI", new[] { "DepartmentID" });
            DropIndex("dbo.KPI", new[] { "DivisionID" });
            AlterColumn("dbo.KPI", "DepartmentID", c => c.Int(nullable: false));
            DropColumn("dbo.KPI", "DivisionID");
            RenameColumn(table: "dbo.KPI", name: "DepartmentID", newName: "OrgStructureID");
            CreateIndex("dbo.KPI", "OrgStructureID");
            AddForeignKey("dbo.KPI", "OrgStructureID", "dbo.OrgStructure", "ID", cascadeDelete: true);
        }
    }
}
