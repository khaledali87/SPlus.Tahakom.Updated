namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_TwoFactorTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TwoFactorAuths",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        Expired = c.DateTime(nullable: false),
                        SMSAuthCode = c.Int(nullable: false),
                        MobileNumber = c.String(nullable: false),
                        Counter = c.Int(nullable: false),
                        UserName = c.String(nullable: false),
                        Token = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TwoFactorAuths");
        }
    }
}
