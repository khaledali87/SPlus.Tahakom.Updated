namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConfigurationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Configurations", "Key", c => c.String());
            AlterColumn("dbo.Configurations", "Value", c => c.String());
            AlterColumn("dbo.Configurations", "Modified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Configurations", "Created", c => c.DateTime(nullable: false));
            DropColumn("dbo.Configurations", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Configurations", "Title", c => c.String(maxLength: 255));
            AlterColumn("dbo.Configurations", "Created", c => c.DateTime());
            AlterColumn("dbo.Configurations", "Modified", c => c.DateTime());
            AlterColumn("dbo.Configurations", "Value", c => c.String(maxLength: 255));
            DropColumn("dbo.Configurations", "Key");
        }
    }
}
