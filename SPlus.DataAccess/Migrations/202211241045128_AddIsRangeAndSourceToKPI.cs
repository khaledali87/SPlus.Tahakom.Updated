namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsRangeAndSourceToKPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "IsRange", c => c.Boolean(nullable: false));
            AddColumn("dbo.KPI", "Source", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPI", "Source");
            DropColumn("dbo.KPI", "IsRange");
        }
    }
}
