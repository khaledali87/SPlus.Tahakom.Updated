namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddValue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Values",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false, maxLength: 255),
                        ArabicName = c.String(nullable: false, maxLength: 255),
                        Order = c.Int(nullable: false),
                        StrategyID = c.Int(nullable: false),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Strategy", t => t.StrategyID, cascadeDelete: true)
                .Index(t => t.StrategyID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Values", "StrategyID", "dbo.Strategy");
            DropIndex("dbo.Values", new[] { "StrategyID" });
            DropTable("dbo.Values");
        }
    }
}
