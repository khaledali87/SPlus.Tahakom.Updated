namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeletedOnUserAndRenameAllowLockPropertyOnKPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "ManualUnLock", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "Deleted", c => c.Boolean(nullable: false));
            DropColumn("dbo.KPI", "ManualLock");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPI", "ManualLock", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "Deleted");
            DropColumn("dbo.KPI", "ManualUnLock");
        }
    }
}
