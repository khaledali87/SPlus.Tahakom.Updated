namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrgStructureAbbreviation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrgStructure", "Abbreviation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrgStructure", "Abbreviation");
        }
    }
}
