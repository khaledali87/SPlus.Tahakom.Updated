namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWeightToOrgStructure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrgStructure", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrgStructure", "Weight");
        }
    }
}
