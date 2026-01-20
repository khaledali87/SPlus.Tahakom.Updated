namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KPIType", "ID", "dbo.Workflow");
            DropIndex("dbo.KPIType", new[] { "ID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.KPIType", "ID");
            AddForeignKey("dbo.KPIType", "ID", "dbo.Workflow", "ID");
        }
    }
}
