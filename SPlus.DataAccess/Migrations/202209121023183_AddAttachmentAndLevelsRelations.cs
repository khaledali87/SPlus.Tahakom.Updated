namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentAndLevelsRelations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Attachements", "KPI_ID", "dbo.KPI");
            DropForeignKey("dbo.Attachements", "Perspective_ID", "dbo.Perspective");
            DropForeignKey("dbo.Attachements", "Theme_ID", "dbo.Theme");
            DropIndex("dbo.Attachements", new[] { "KPI_ID" });
            DropIndex("dbo.Attachements", new[] { "Perspective_ID" });
            DropIndex("dbo.Attachements", new[] { "Theme_ID" });
            AddColumn("dbo.Attachements", "RelatedItemID", c => c.Int(nullable: false));
            DropColumn("dbo.Attachements", "KPI_ID");
            DropColumn("dbo.Attachements", "Perspective_ID");
            DropColumn("dbo.Attachements", "Theme_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attachements", "Theme_ID", c => c.Int());
            AddColumn("dbo.Attachements", "Perspective_ID", c => c.Int());
            AddColumn("dbo.Attachements", "KPI_ID", c => c.Int());
            DropColumn("dbo.Attachements", "RelatedItemID");
            CreateIndex("dbo.Attachements", "Theme_ID");
            CreateIndex("dbo.Attachements", "Perspective_ID");
            CreateIndex("dbo.Attachements", "KPI_ID");
            AddForeignKey("dbo.Attachements", "Theme_ID", "dbo.Theme", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Attachements", "Perspective_ID", "dbo.Perspective", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Attachements", "KPI_ID", "dbo.KPI", "ID");
        }
    }
}
