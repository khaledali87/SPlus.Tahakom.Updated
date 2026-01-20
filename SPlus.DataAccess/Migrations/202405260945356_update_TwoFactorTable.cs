namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_TwoFactorTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TwoFactorAuths", "MobileNumber", c => c.String());
            AlterColumn("dbo.TwoFactorAuths", "Token", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TwoFactorAuths", "Token", c => c.String(nullable: false));
            AlterColumn("dbo.TwoFactorAuths", "MobileNumber", c => c.String(nullable: false));
        }
    }
}
