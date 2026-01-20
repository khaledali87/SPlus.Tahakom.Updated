namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActionByToRequestSteps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestStep", "ActionBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequestStep", "ActionBy");
        }
    }
}
