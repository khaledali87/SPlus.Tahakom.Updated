namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnlockDateToActivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "UnlockDate", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activity", "UnlockDate");
        }
    }
}
