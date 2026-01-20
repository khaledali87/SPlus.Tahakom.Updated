namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_KPIHistoryTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPIHistories", "SumCumulativePerformance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.KPIHistories", "AverageCumulativePerformance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIHistories", "AverageCumulativePerformance");
            DropColumn("dbo.KPIHistories", "SumCumulativePerformance");
        }
    }
}
