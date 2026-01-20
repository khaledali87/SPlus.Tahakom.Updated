namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKPISourceField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "KPISource", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPI", "KPISource");
        }
    }
}
