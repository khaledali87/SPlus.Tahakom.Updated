namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsVerifiedFlagtoRequestStep : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestStep", "IsVerified", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RequestStep", "IsVerified");
        }
    }
}
