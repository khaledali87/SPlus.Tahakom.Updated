namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteActivityStatuses : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActivityThreshold", "Code", "dbo.ActivityStatus");
            AddForeignKey("dbo.ActivityThreshold", "Code", "dbo.Status", "Code", cascadeDelete: true);
            DropTable("dbo.ActivityStatus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ActivityStatus",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 3),
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Color = c.String(nullable: false),
                        Order = c.Int(),
                    })
                .PrimaryKey(t => t.Code);
            
            DropForeignKey("dbo.ActivityThreshold", "Code", "dbo.Status");
            AddForeignKey("dbo.ActivityThreshold", "Code", "dbo.ActivityStatus", "Code", cascadeDelete: true);
        }
    }
}
