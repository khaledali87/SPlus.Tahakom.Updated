namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRequestStepAndKPIForm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RequestStep", "QualityType", c => c.Int(nullable: false));
            DropColumn("dbo.RequestStep", "IsVerified");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RequestStep", "IsVerified", c => c.Boolean());
            DropColumn("dbo.RequestStep", "QualityType");
        }
    }
}
