namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPerspectiveTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Perspective",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        StrategyID = c.Int(nullable: false),
                        Created = c.DateTime(),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Strategy", t => t.StrategyID)
                .Index(t => t.StrategyID);
            
            AddColumn("dbo.KPI", "PerspectiveID", c => c.Int());
            CreateIndex("dbo.KPI", "PerspectiveID");
            AddForeignKey("dbo.KPI", "PerspectiveID", "dbo.Perspective", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Perspective", "StrategyID", "dbo.Strategy");
            DropForeignKey("dbo.KPI", "PerspectiveID", "dbo.Perspective");
            DropIndex("dbo.Perspective", new[] { "StrategyID" });
            DropIndex("dbo.KPI", new[] { "PerspectiveID" });
            DropColumn("dbo.KPI", "PerspectiveID");
            DropTable("dbo.Perspective");
        }
    }
}
