namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateIsVerifiedFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.KPI", "IsVerified", c => c.Boolean());
            DropColumn("dbo.KPIMeasures", "IsVerified");
        }
        
        public override void Down()
        {
            AddColumn("dbo.KPIMeasures", "IsVerified", c => c.Boolean());
            DropColumn("dbo.KPI", "IsVerified");
        }
    }
}
