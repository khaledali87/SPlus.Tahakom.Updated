namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStrategyType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Strategy", "StrategyType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Strategy", "StrategyType", c => c.String(nullable: false, maxLength: 255));
        }
    }
}
