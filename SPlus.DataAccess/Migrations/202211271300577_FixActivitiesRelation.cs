namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixActivitiesRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ParameterValues", "ActivityMeasure_ID", "dbo.ActivityMeasures");
            DropIndex("dbo.ParameterValues", new[] { "ActivityMeasure_ID" });
            DropColumn("dbo.ParameterValues", "ActivityMeasure_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ParameterValues", "ActivityMeasure_ID", c => c.Int());
            CreateIndex("dbo.ParameterValues", "ActivityMeasure_ID");
            AddForeignKey("dbo.ParameterValues", "ActivityMeasure_ID", "dbo.ActivityMeasures", "ID");
        }
    }
}
