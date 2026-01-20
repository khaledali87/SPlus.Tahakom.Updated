namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBusinessUnitWeight : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "BusinessUnitWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPI", "BusinessUnitWeight");
        }
    }
}
