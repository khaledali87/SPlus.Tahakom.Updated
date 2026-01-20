namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixKPIRequiredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KPI", "EnglishDescription", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPI", "EnglishDescription", c => c.String(nullable: false));
        }
    }
}
