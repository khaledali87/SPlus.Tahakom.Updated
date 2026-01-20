namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveStatusToKPIAndActivity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Activity", "Status");
            DropColumn("dbo.KPI", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPI", "Status", c => c.String());
            AddColumn("dbo.Activity", "Status", c => c.String());
        }
    }
}
