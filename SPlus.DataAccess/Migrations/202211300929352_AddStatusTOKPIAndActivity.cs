namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusTOKPIAndActivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "Status", c => c.String());
            AddColumn("dbo.KPI", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPI", "Status");
            DropColumn("dbo.Activity", "Status");
        }
    }
}
