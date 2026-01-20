namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPerspectiveBackground : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Perspective", "BackgroundColor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Perspective", "BackgroundColor");
        }
    }
}
