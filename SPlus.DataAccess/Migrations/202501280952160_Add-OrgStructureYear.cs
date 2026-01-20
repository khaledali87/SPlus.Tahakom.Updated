namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrgStructureYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrgStructure", "YearString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrgStructure", "YearString");
        }
    }
}
