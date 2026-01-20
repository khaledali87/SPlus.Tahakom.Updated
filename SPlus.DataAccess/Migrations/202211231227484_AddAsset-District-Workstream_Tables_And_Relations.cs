namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAssetDistrictWorkstream_Tables_And_Relations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArabicName = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        EnglishDescription = c.String(nullable: false),
                        ArabicDescription = c.String(),
                        Manager = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        DistrictID = c.Int(nullable: false),
                        ManagerModel_UserName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Districts", t => t.DistrictID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.ManagerModel_UserName)
                .Index(t => t.DistrictID)
                .Index(t => t.ManagerModel_UserName);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArabicName = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        WorkstreamID = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Workstreams", t => t.WorkstreamID, cascadeDelete: true)
                .Index(t => t.WorkstreamID);
            
            CreateTable(
                "dbo.Workstreams",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArabicName = c.String(nullable: false),
                        EnglishName = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assets", "ManagerModel_UserName", "dbo.Users");
            DropForeignKey("dbo.Districts", "WorkstreamID", "dbo.Workstreams");
            DropForeignKey("dbo.Assets", "DistrictID", "dbo.Districts");
            DropIndex("dbo.Districts", new[] { "WorkstreamID" });
            DropIndex("dbo.Assets", new[] { "ManagerModel_UserName" });
            DropIndex("dbo.Assets", new[] { "DistrictID" });
            DropTable("dbo.Workstreams");
            DropTable("dbo.Districts");
            DropTable("dbo.Assets");
        }
    }
}
