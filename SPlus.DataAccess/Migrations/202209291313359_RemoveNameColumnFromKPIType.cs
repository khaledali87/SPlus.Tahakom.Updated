namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveNameColumnFromKPIType : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.KPIType", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPIType", "Name", c => c.String(nullable: false));
        }
    }
}
