namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateIsVerifiedFlagToActivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "IsVerified", c => c.Boolean());
            DropColumn("dbo.ActivityMeasures", "IsVerified");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ActivityMeasures", "IsVerified", c => c.Boolean());
            DropColumn("dbo.Activity", "IsVerified");
        }
    }
}
