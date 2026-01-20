namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsConstantTargetFiled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "IsConstantTargets", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPI", "IsConstantTargets");
        }
    }
}
