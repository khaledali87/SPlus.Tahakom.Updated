namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIconFieldFromKPIType : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.KPIType", "Icon");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPIType", "Icon", c => c.String(nullable: false));
        }
    }
}
