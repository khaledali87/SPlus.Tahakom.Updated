namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateAuditTrailAndExceptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditTrail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ItemID = c.Int(nullable: false),
                        ItemType = c.String(),
                        Action = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AuditTrail");
        }
    }
}
