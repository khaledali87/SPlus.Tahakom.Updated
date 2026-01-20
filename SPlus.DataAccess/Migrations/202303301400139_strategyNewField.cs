namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class strategyNewField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Strategy", "IsOperational", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Strategy", "IsOperational");
        }
    }
}
