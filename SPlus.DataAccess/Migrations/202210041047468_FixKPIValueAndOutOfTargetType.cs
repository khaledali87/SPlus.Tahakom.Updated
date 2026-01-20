namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixKPIValueAndOutOfTargetType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KPI", "Baseline", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KPI", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "Value", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "OutOfTarget", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPIMeasures", "OutOfTarget", c => c.Decimal(precision: 18, scale: 0));
            AlterColumn("dbo.KPIMeasures", "Value", c => c.Decimal(precision: 18, scale: 0));
            AlterColumn("dbo.KPIMeasures", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.KPI", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.KPI", "Baseline", c => c.Decimal(nullable: false, precision: 18, scale: 0));
        }
    }
}
