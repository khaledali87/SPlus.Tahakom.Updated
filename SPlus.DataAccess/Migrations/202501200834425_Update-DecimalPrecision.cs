namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDecimalPrecision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KPI", "Baseline", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("dbo.KPI", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("dbo.KPI", "BusinessUnitWeight", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("dbo.KPIMeasures", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("dbo.KPIMeasures", "Value", c => c.Decimal(precision: 18, scale: 6));
            AlterColumn("dbo.KPIMeasures", "OutOfTarget", c => c.Decimal(precision: 18, scale: 6));
            AlterColumn("dbo.KPIMeasures", "AccumulutiveOutOfTarget", c => c.Decimal(precision: 18, scale: 6));
            AlterColumn("dbo.KPIMeasures", "AccumulutiveValue", c => c.Decimal(precision: 18, scale: 6));
            AlterColumn("dbo.KPIMeasures", "AccumulutiveTarget", c => c.Decimal(precision: 18, scale: 6));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPIMeasures", "AccumulutiveTarget", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "AccumulutiveValue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "AccumulutiveOutOfTarget", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "OutOfTarget", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "Value", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.KPIMeasures", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KPI", "BusinessUnitWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KPI", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KPI", "Baseline", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
