namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeUnitDetailsOptionalforKPI : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KPI", "EnglishUnitDetails", c => c.String());
            AlterColumn("dbo.KPI", "ArabicUnitDetails", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPI", "ArabicUnitDetails", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "EnglishUnitDetails", c => c.String(nullable: false));
        }
    }
}
