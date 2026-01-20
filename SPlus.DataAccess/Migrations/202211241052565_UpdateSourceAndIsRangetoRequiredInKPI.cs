namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSourceAndIsRangetoRequiredInKPI : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KPI", "Source", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPI", "Source", c => c.String());
        }
    }
}
