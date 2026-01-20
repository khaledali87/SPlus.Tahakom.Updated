namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAttachmentTypePropertyToOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Attachements", "Type", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Attachements", "Type", c => c.String(nullable: false));
        }
    }
}
