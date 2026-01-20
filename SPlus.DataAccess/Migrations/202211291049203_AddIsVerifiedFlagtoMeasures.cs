namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsVerifiedFlagtoMeasures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityMeasures", "IsVerified", c => c.Boolean());
            AddColumn("dbo.KPIMeasures", "IsVerified", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIMeasures", "IsVerified");
            DropColumn("dbo.ActivityMeasures", "IsVerified");
        }
    }
}
