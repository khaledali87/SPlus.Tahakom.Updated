namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPIMeasures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPIMeasures", "AccumulutiveOutOfTarget", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.KPIMeasures", "AccumulutiveValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.KPIMeasures", "AccumulutiveTarget", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.KPIMeasures", "AccumulutiveStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIMeasures", "AccumulutiveStatus");
            DropColumn("dbo.KPIMeasures", "AccumulutiveTarget");
            DropColumn("dbo.KPIMeasures", "AccumulutiveValue");
            DropColumn("dbo.KPIMeasures", "AccumulutiveOutOfTarget");
        }
    }
}
