namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateKPIModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure");
            DropIndex("dbo.KPI", new[] { "DivisionID" });
            AlterColumn("dbo.KPI", "EnglishName", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "ArabicName", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "Formula", c => c.String());
            AlterColumn("dbo.KPI", "DataSource", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "UnitOfMeasure", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "Polarity", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "Frequency", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "StartDate", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "Years", c => c.Int(nullable: false));
            AlterColumn("dbo.KPI", "Direction", c => c.String());
            AlterColumn("dbo.KPI", "EnglishUnitDetails", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "ArabicUnitDetails", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "EnglishEquation", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "ArabicEquation", c => c.String(nullable: false));
            AlterColumn("dbo.KPI", "DivisionID", c => c.Int());
            CreateIndex("dbo.KPI", "DivisionID");
            AddForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure");
            DropIndex("dbo.KPI", new[] { "DivisionID" });
            AlterColumn("dbo.KPI", "DivisionID", c => c.Int(nullable: false));
            AlterColumn("dbo.KPI", "ArabicEquation", c => c.String());
            AlterColumn("dbo.KPI", "EnglishEquation", c => c.String());
            AlterColumn("dbo.KPI", "ArabicUnitDetails", c => c.String(maxLength: 255));
            AlterColumn("dbo.KPI", "EnglishUnitDetails", c => c.String(maxLength: 255));
            AlterColumn("dbo.KPI", "Direction", c => c.String(maxLength: 255));
            AlterColumn("dbo.KPI", "Years", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPI", "StartDate", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPI", "Frequency", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPI", "Polarity", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPI", "UnitOfMeasure", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPI", "DataSource", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPI", "Formula", c => c.String(maxLength: 255));
            AlterColumn("dbo.KPI", "ArabicName", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPI", "EnglishName", c => c.String(nullable: false, maxLength: 255));
            CreateIndex("dbo.KPI", "DivisionID");
            AddForeignKey("dbo.KPI", "DivisionID", "dbo.OrgStructure", "ID", cascadeDelete: true);
        }
    }
}
