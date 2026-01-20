namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRequestStepActionBy : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RequestStep", "ActionBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RequestStep", "ActionBy", c => c.String());
        }
    }
}
