namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartemntToKPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "DepartmentID", c => c.Int());
            CreateIndex("dbo.KPI", "DepartmentID");
            AddForeignKey("dbo.KPI", "DepartmentID", "dbo.OrgStructure", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KPI", "DepartmentID", "dbo.OrgStructure");
            DropIndex("dbo.KPI", new[] { "DepartmentID" });
            DropColumn("dbo.KPI", "DepartmentID");
        }
    }
}
