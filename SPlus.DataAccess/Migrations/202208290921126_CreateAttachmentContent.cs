namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateAttachmentContent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AttachmentContents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Content = c.Binary(nullable: false),
                        AttachmentID = c.Int(nullable: false),
                        Attachment_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Attachements", t => t.Attachment_ID, cascadeDelete: true)
                .Index(t => t.Attachment_ID);
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attachements", "Content", c => c.Binary(nullable: false));
            DropForeignKey("dbo.AttachmentContents", "Attachment_ID", "dbo.Attachements");
            DropIndex("dbo.AttachmentContents", new[] { "Attachment_ID" });
            DropTable("dbo.AttachmentContents");
        }
    }
}
