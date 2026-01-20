namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentRelationToPerspectiveAndTheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachements", "Attachement_ID", c => c.Int());
            RenameColumn(table: "dbo.Attachements", name: "Attachement_ID", newName: "Perspective_ID");
            AddColumn("dbo.Attachements", "Attachement_ID", c => c.Int());
            RenameColumn(table: "dbo.Attachements", name: "Attachement_ID", newName: "Theme_ID");
            CreateIndex("dbo.Attachements", "Perspective_ID");
            CreateIndex("dbo.Attachements", "Theme_ID");
            AddForeignKey("dbo.Attachements", "Perspective_ID", "dbo.Perspective", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Attachements", "Theme_ID", "dbo.Theme", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.Theme", "Attachement_ID", c => c.Int());
            AddColumn("dbo.Perspective", "Attachement_ID", c => c.Int());
            DropForeignKey("dbo.Attachements", "Theme_ID", "dbo.Theme");
            DropForeignKey("dbo.Attachements", "Perspective_ID", "dbo.Perspective");
            DropIndex("dbo.Attachements", new[] { "Theme_ID" });
            DropIndex("dbo.Attachements", new[] { "Perspective_ID" });
            RenameColumn(table: "dbo.Attachements", name: "Theme_ID", newName: "Attachement_ID");
            RenameColumn(table: "dbo.Attachements", name: "Perspective_ID", newName: "Attachement_ID");
            CreateIndex("dbo.Theme", "Attachement_ID");
            CreateIndex("dbo.Perspective", "Attachement_ID");
            AddForeignKey("dbo.Theme", "Attachement_ID", "dbo.Attachements", "ID");
            AddForeignKey("dbo.Perspective", "Attachement_ID", "dbo.Attachements", "ID");
        }
    }
}
