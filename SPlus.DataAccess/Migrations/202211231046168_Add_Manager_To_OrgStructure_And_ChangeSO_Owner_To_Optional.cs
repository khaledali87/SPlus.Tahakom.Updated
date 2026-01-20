namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Manager_To_OrgStructure_And_ChangeSO_Owner_To_Optional : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StrategicObjective", new[] { "Owner" });
            AddColumn("dbo.OrgStructure", "Manager", c => c.String(maxLength: 128));
            AlterColumn("dbo.StrategicObjective", "Owner", c => c.String(maxLength: 128));
            CreateIndex("dbo.OrgStructure", "Manager");
            CreateIndex("dbo.StrategicObjective", "Owner");
            AddForeignKey("dbo.OrgStructure", "Manager", "dbo.Users", "UserName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrgStructure", "Manager", "dbo.Users");
            DropIndex("dbo.StrategicObjective", new[] { "Owner" });
            DropIndex("dbo.OrgStructure", new[] { "Manager" });
            AlterColumn("dbo.StrategicObjective", "Owner", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.OrgStructure", "Manager");
            CreateIndex("dbo.StrategicObjective", "Owner");
        }
    }
}
