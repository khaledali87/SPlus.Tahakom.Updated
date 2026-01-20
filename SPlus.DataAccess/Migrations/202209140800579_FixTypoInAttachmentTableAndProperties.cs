namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTypoInAttachmentTableAndProperties : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Attachements", newName: "Attachments");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Attachments", newName: "Attachements");
        }
    }
}
