namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeKPIRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestStep", "IsCancelled", c => c.Boolean(nullable: false));
            AddColumn("dbo.KPIMeasures", "CalculationMethod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIMeasures", "CalculationMethod");
            DropColumn("dbo.RequestStep", "IsCancelled");
        }
    }
}
