namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsCompletedFlagForActivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "IsCompleted", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activity", "IsCompleted");
        }
    }
}
