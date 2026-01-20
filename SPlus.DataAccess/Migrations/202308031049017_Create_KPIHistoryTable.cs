namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_KPIHistoryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KPIHistories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KPIID = c.Int(nullable: false),
                        Baseline = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        OutOfTarget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HistoryDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.KPIHistories");
        }
    }
}
