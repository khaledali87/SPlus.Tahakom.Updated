namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPIDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "Code", c => c.String());
            AddColumn("dbo.KPI", "DepartmentalWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.KPI", "IsRange");
            DropColumn("dbo.KPI", "IsVerified");
            DropColumn("dbo.KPIMeasures", "MaxTarget");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPIMeasures", "MaxTarget", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.KPI", "IsVerified", c => c.Boolean());
            AddColumn("dbo.KPI", "IsRange", c => c.Boolean(nullable: false));
            DropColumn("dbo.KPI", "DepartmentalWeight");
            DropColumn("dbo.KPI", "Code");
        }
    }
}
