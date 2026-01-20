namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManualLockColumnToKPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "ManualLock", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPI", "ManualLock");
        }
    }
}
