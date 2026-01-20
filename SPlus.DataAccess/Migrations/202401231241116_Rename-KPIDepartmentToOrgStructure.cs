namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameKPIDepartmentToOrgStructure : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.KPI", name: "DepartmentID", newName: "OrgStructureID");
            RenameIndex(table: "dbo.KPI", name: "IX_DepartmentID", newName: "IX_OrgStructureID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.KPI", name: "IX_OrgStructureID", newName: "IX_DepartmentID");
            RenameColumn(table: "dbo.KPI", name: "OrgStructureID", newName: "DepartmentID");
        }
    }
}
