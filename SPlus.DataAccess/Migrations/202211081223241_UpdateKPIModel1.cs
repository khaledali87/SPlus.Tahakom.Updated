namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPIModel1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KPI", "EnglishDescription", c => c.String());
            AlterColumn("dbo.KPI", "EnglishEquation", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPI", "EnglishEquation", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "EnglishDescription", c => c.String(nullable: false));
        }
    }
}
