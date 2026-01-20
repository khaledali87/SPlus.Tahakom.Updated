namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateActivityRangeTypeToDecimal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityMeasures", "Range", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ActivityMeasures", "RangeDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ActivityMeasures", "RangeDate", c => c.DateTime());
            DropColumn("dbo.ActivityMeasures", "Range");
        }
    }
}
