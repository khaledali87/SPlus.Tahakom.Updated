namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DelegationRelationswithUser_Fix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Delegation", "CreatedBy", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Delegation", "ModifiedBy", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.Delegation", "CreatedBy");
            CreateIndex("dbo.Delegation", "ModifiedBy");
            AddForeignKey("dbo.Delegation", "CreatedBy", "dbo.Users", "UserName");
            AddForeignKey("dbo.Delegation", "ModifiedBy", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Delegation", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Delegation", "CreatedBy", "dbo.Users");
            DropIndex("dbo.Delegation", new[] { "ModifiedBy" });
            DropIndex("dbo.Delegation", new[] { "CreatedBy" });
            AlterColumn("dbo.Delegation", "ModifiedBy", c => c.String(maxLength: 255));
            AlterColumn("dbo.Delegation", "CreatedBy", c => c.String(maxLength: 255));
        }
    }
}
