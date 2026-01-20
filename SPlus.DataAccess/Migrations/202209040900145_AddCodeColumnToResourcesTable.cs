namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodeColumnToResourcesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "Code", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Resources", "PropertyValue", c => c.Int());
            AlterColumn("dbo.Resources", "LevelID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Resources", "LevelID", c => c.Int(nullable: false));
            AlterColumn("dbo.Resources", "PropertyValue", c => c.Int(nullable: false));
            DropColumn("dbo.Resources", "Code");
        }
    }
}
