namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixStartDateType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Activity", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.KPI", "StartDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPI", "StartDate", c => c.String(nullable: false));
            AlterColumn("dbo.Activity", "StartDate", c => c.String(nullable: false));
        }
    }
}
