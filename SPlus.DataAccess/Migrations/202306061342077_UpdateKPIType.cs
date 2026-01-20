namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPIType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPIType", "IsDepartmental", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIType", "IsDepartmental");
        }
    }
}
