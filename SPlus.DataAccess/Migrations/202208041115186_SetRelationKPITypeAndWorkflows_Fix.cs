namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetRelationKPITypeAndWorkflows_Fix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType");
            DropPrimaryKey("dbo.KPIType");
            AddPrimaryKey("dbo.KPIType", "ID");
            AddForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType", "ID");
            AddForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType");
            DropForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType");
            DropPrimaryKey("dbo.KPIType");
            AddPrimaryKey("dbo.KPIType", "KPITypeID");
            AddForeignKey("dbo.Workflow", "KPITypeID", "dbo.KPIType", "KPITypeID");
            AddForeignKey("dbo.KPIThreshold", "KPITypeID", "dbo.KPIType", "KPITypeID");
        }
    }
}
