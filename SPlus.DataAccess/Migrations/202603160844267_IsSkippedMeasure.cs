namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsSkippedMeasure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPIMeasures", "IsSkipped", c => c.Boolean(nullable: false , defaultValue : false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.KPIMeasures", "IsSkipped");
        }
    }
}
