namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditGroupsIsEditableAndIsDeletable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Groups", "IsDeletable");
            DropColumn("dbo.Groups", "IsEditable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Groups", "IsEditable", c => c.Boolean());
            AddColumn("dbo.Groups", "IsDeletable", c => c.Boolean());
        }
    }
}
