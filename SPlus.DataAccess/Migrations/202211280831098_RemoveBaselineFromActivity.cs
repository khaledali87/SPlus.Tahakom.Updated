namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBaselineFromActivity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Activity", "Baseline");
            DropColumn("dbo.Activity", "BaselineDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Activity", "BaselineDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Activity", "Baseline", c => c.DateTime(nullable: false));
        }
    }
}
