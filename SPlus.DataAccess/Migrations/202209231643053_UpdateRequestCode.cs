namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRequestCode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Request",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WorkflowID = c.Int(nullable: false),
                        CreatedBy = c.String(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Form = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        RelatedRequestID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.RequestStep",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RequestID = c.Int(nullable: false),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        IsGroup = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        Approver = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        ActionBy = c.String(),
                        Comments = c.String(),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        ActionByModel_UserName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.ActionByModel_UserName)
                .ForeignKey("dbo.Request", t => t.RequestID, cascadeDelete: true)
                .Index(t => t.RequestID)
                .Index(t => t.ActionByModel_UserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestStep", "RequestID", "dbo.Request");
            DropForeignKey("dbo.RequestStep", "ActionByModel_UserName", "dbo.Users");
            DropIndex("dbo.RequestStep", new[] { "ActionByModel_UserName" });
            DropIndex("dbo.RequestStep", new[] { "RequestID" });
            DropTable("dbo.RequestStep");
            DropTable("dbo.Request");
        }
    }
}
