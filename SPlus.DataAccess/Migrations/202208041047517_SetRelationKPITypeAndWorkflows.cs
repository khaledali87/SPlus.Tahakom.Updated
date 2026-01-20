namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetRelationKPITypeAndWorkflows : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType");
            DropPrimaryKey("dbo.KPIType");
            AddColumn("dbo.KPIType", "KPITypeID", c => c.Int(nullable: false));
            AddColumn("dbo.Workflow", "KPITypeID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.KPIType", "KPITypeID");
            CreateIndex("dbo.Workflow", "KPITypeID");
            AddForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType", "KPITypeID");
            AddForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType", "KPITypeID");
            DropColumn("dbo.Workflow", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Workflow", "Type", c => c.Int(nullable: false));
            DropForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType");
            DropIndex("dbo.Workflow", new[] { "KPITypeID" });
            DropPrimaryKey("dbo.KPIType");
            DropColumn("dbo.Workflow", "KPITypeID");
            DropColumn("dbo.KPIType", "KPITypeID");
            AddPrimaryKey("dbo.KPIType", "ID");
            AddForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType", "ID");
        }
    }
}
