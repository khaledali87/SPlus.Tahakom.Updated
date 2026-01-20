namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBusinessUnitWeightTOHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPIHistories", "BusinessUnitWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIHistories", "BusinessUnitWeight");
        }
    }
}
