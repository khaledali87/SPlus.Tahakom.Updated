namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPIDetails1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.KPI", "Source");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPI", "Source", c => c.String(nullable: false));
        }
    }
}
