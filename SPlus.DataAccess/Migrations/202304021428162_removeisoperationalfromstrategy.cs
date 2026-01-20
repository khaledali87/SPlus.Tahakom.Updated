namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeisoperationalfromstrategy : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Strategy", "IsOperational");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Strategy", "IsOperational", c => c.Boolean(nullable: false));
        }
    }
}
