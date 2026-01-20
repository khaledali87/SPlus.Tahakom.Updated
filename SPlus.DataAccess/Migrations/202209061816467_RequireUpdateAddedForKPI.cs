namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequireUpdateAddedForKPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "RequireUpdate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPI", "RequireUpdate");
        }
    }
}
