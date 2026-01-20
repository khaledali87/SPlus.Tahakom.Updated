namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProgressAndStatusToAssets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "Progress", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Assets", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "Status");
            DropColumn("dbo.Assets", "Progress");
        }
    }
}
