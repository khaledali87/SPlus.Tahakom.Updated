namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCalculationMethodToKPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "CalculationMethod", c => c.Int(nullable: false));
            AddColumn("dbo.KPIMeasures", "MaxTarget", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIMeasures", "MaxTarget");
            DropColumn("dbo.KPI", "CalculationMethod");
        }
    }
}
