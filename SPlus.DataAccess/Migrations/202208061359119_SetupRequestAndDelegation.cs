namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetupRequestAndDelegation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Delegation", "FromUser", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Delegation", "ToUser", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.Delegation", "FromUser");
            CreateIndex("dbo.Delegation", "ToUser");
            AddForeignKey("dbo.Delegation", "FromUser", "dbo.Users", "UserName");
            AddForeignKey("dbo.Delegation", "ToUser", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Delegation", "ToUser", "dbo.Users");
            DropForeignKey("dbo.Delegation", "FromUser", "dbo.Users");
            DropIndex("dbo.Delegation", new[] { "ToUser" });
            DropIndex("dbo.Delegation", new[] { "FromUser" });
            AlterColumn("dbo.Delegation", "ToUser", c => c.String(maxLength: 255));
            AlterColumn("dbo.Delegation", "FromUser", c => c.String(maxLength: 255));
        }
    }
}
