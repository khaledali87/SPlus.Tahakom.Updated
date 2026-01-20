namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMatrixRelations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "Name", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.Resources", "IsScreen", c => c.Boolean(nullable: false));
            AddColumn("dbo.Resources", "PropertyName", c => c.String());
            AddColumn("dbo.Resources", "PropertyValue", c => c.Int(nullable: false));
            AddColumn("dbo.Resources", "LevelID", c => c.Int(nullable: false));
            CreateIndex("dbo.Matrix", "GroupID");
            CreateIndex("dbo.Matrix", "ResourceID");
            CreateIndex("dbo.Matrix", "RoleID");
            AddForeignKey("dbo.Matrix", "ResourceID", "dbo.Resources", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Matrix", "RoleID", "dbo.Roles", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Matrix", "GroupID", "dbo.Groups", "ID", cascadeDelete: true);
            DropColumn("dbo.Resources", "Title");
            DropColumn("dbo.Resources", "IsKPI");
            DropColumn("dbo.Resources", "TypeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Resources", "TypeID", c => c.Int(nullable: false));
            AddColumn("dbo.Resources", "IsKPI", c => c.Boolean(nullable: false));
            AddColumn("dbo.Resources", "Title", c => c.String(nullable: false, maxLength: 255));
            DropForeignKey("dbo.Matrix", "GroupID", "dbo.Groups");
            DropForeignKey("dbo.Matrix", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.Matrix", "ResourceID", "dbo.Resources");
            DropIndex("dbo.Matrix", new[] { "RoleID" });
            DropIndex("dbo.Matrix", new[] { "ResourceID" });
            DropIndex("dbo.Matrix", new[] { "GroupID" });
            DropColumn("dbo.Resources", "LevelID");
            DropColumn("dbo.Resources", "PropertyValue");
            DropColumn("dbo.Resources", "PropertyName");
            DropColumn("dbo.Resources", "IsScreen");
            DropColumn("dbo.Resources", "Name");
        }
    }
}
